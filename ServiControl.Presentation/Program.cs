using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Application.Services;
using ServiControl.Infrastructure;
using ServiControl.Presentation.Security;

// Modulo: API
// Capa: Presentation
// Responsabilidad: Configura servicios HTTP, autenticacion, Swagger y dependencias.
// Nota: La configuracion se lee desde appsettings o variables de entorno, compatible con Azure App Service.
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//servicio para los controladores
builder.Services.AddControllers();

//servicio para que swagger lea los endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //definimos como va a usar el token (es para cuando usemos [autorize] en swagger boton arriba a la derecha)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token con el formato: Bearer {token}"
    });
    //aca usamos el esquema anterior OpenApiSecurityRequrement, el que acabamos de definir arriba.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

//las politicas de CORS permiten que un frontend consuma nuestra API. en nuestras politicas validamos quien queremos que consuma nuestra API (desde el navegador)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        //en entorno de desarrollo puede consumirla desde cualquier lugar (swagger por ejemplo)
        if (builder.Environment.IsDevelopment())
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();

            return;
        }

        //aca aplicamos la politica CORS fuera del entorno de desarrollo
        var allowedOrigins = builder.Configuration
        //getsection toma toda la seccion cors
        //getChildren los hijos de esa seccion
        
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(origin => origin.Value)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin!)
            .ToArray();

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Jwt:Key debe configurarse como variable de entorno en Azure: Jwt__Key.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("La clave JWT no esta configurada.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Si el rol cambio o el usuario fue eliminado, el JWT anterior deja de ser valido.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var usuarioIdValue = context.Principal?.FindFirstValue(AuthClaimTypes.UsuarioId);
                var tokenRole = context.Principal?.FindFirstValue(ClaimTypes.Role);

                if (!int.TryParse(usuarioIdValue, out var usuarioId) || usuarioId <= 0)
                {
                    context.Fail("El token no contiene un UsuarioId valido.");
                    return;
                }

                var usuarioRepository = context.HttpContext.RequestServices
                    .GetRequiredService<IUsuarioRepository>();
                var usuario = await usuarioRepository.GetByIdAsync(usuarioId, context.HttpContext.RequestAborted);

                if (usuario is null || !string.Equals(
                        usuario.Rol.ToString(),
                        tokenRole,
                        StringComparison.Ordinal))
                {
                    context.Fail("El usuario o su rol cambiaron. Debe iniciar sesion nuevamente.");
                }
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITrabajoService, TrabajoService>();
builder.Services.AddScoped<ICostoService, CostoService>();
builder.Services.AddScoped<IMetricaService, MetricaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IOperationalUserResolver, OperationalUserResolver>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (args.Contains("create-admin", StringComparer.OrdinalIgnoreCase))
{
    await CreateAdminFromCommandAsync(app.Services);
    return;
}

if (app.Environment.IsDevelopment())
{
    // Swagger queda activo solo en Development para documentar y probar la API durante desarrollo.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task CreateAdminFromCommandAsync(IServiceProvider services)
{
    Console.WriteLine("Creacion controlada de un administrador");
    Console.Write("Nombre: ");
    var nombre = Console.ReadLine() ?? string.Empty;

    Console.Write("Email: ");
    var email = Console.ReadLine() ?? string.Empty;

    var password = ReadPassword("Password: ");

    using var scope = services.CreateScope();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    try
    {
        var admin = await authService.CreateAdminAsync(
            new CreateAdminRequestDto(nombre, email, password));

        Console.WriteLine($"Administrador creado correctamente: {admin.Email}");
    }
    catch (ArgumentException ex)
    {
        Console.Error.WriteLine($"No se pudo crear el administrador: {ex.Message}");
        Environment.ExitCode = 1;
    }
    catch (InvalidOperationException ex)
    {
        Console.Error.WriteLine($"No se pudo crear el administrador: {ex.Message}");
        Environment.ExitCode = 1;
    }
}

static string ReadPassword(string prompt)
{
    Console.Write(prompt);
    var password = new StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return password.ToString();
        }

        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password.Length--;
            Console.Write("\b \b");
            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write('*');
        }
    }
}
