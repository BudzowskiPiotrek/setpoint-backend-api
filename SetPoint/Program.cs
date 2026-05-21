using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SetPoint.BLL._0.Sync;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._03.BodyMeasurementsManagement;
using SetPoint.BLL._03.BodyMeasurementsManagement.Dto;
using SetPoint.BLL._04.ExercisesManagement;
using SetPoint.BLL._05.MuscleGroupsManagement;
using SetPoint.BLL._06.ExerciseMuscleManagement;
using SetPoint.BLL._07.RoutineRequestManagement;
using SetPoint.BLL._07.RoutinesManagement;
using SetPoint.BLL._08.RoutineExercisesManagement;
using SetPoint.BLL._09.WorkoutSessionsManagement;
using SetPoint.BLL._1.Security;
using SetPoint.BLL._10.WorkoutExercisesManagement;
using SetPoint.BLL._11.ExerciseSetsManagement;
using System.Text;
using System.Threading.RateLimiting;
//---------------------------------------------------------------------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);
//------------------------------------------------------------------------------------------ Add services to the container. -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//------------------------------------------------------------------------------------------ Add CORS policy ----------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite cualquier origen (tu IP de móvil)
              .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();  // Permite cualquier cabecera (tokens, etc.)
    });

    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("*************") // POR SI HAGO PAGINA WEB TAMBIEN, REEMPLAZA CON TU DOMINIO REAL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
//------------------------------------------------------------------------------------------ Dependency Injection for BLL and Services --------
builder.Services.AddScoped<IUserBll, UserBll>();
builder.Services.AddScoped<IRoutineBll, RoutineBll>();
builder.Services.AddScoped<IExercisesBll, ExercisesBll>();
builder.Services.AddScoped<IMuscleGroupBll, MuscleGroupBll>();
builder.Services.AddScoped<IExerciseSetsBll, ExerciseSetsBll>();
builder.Services.AddScoped<IUserRelationBll, UserRelationBll>();
builder.Services.AddScoped<IRoutineRequestBll, RoutineRequestBll>();
builder.Services.AddScoped<IWorkoutSessionsBll, WorkoutSessionsBll>();
builder.Services.AddScoped<IRoutineExercisesBll, RoutineExercisesBll>();
builder.Services.AddScoped<IBodyMeasurementsBll, BodyMeasurementsBll>();
builder.Services.AddScoped<IWorkoutExercisesBll, WorkoutExercisesBll>();
builder.Services.AddScoped<IExerciseMuscleGroupBll, ExerciseMuscleGroupBll>();
//------------------------------------------------------------------------------------------ Dependency Injection Services --------------------
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
//------------------------------------------------------------------------------------------ Configure JWT Authentication ---------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});
//------------------------------------------------------------------------------------------ Configure Forwarded Headers for Reverse Proxy ----
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
//------------------------------------------------------------------------------------------ Configure Rate Limiting --------------------------
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("SincronizacionLenta", context =>
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User?.Identity?.Name ?? remoteIp,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
//---------------------------------------------------------------------------------------------------------------------------------------------
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(8000);
//});
var app = builder.Build();
app.UseForwardedHeaders();
//------------------------------------------------------------------------------------------ Configure the HTTP request pipeline. -------------
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseCors("ProductionPolicy");
    app.UseHttpsRedirection();
}
//---------------------------------------------------------------------------------------------------------------------------------------------
app.UseRouting(); ///////// Asegura que el middleware de enrutamiento se ejecute antes de la autenticación y autorización
app.UseRateLimiter(); ///// Asegura que el middleware de limitación de tasa se ejecute antes de la autorización para proteger los endpoints
app.UseAuthentication(); // Asegura que el middleware de autenticación se ejecute antes de la autorización
app.UseAuthorization(); /// Asegura que el middleware de autorización se ejecute después de la autenticación y limitación de tasa
app.MapControllers(); ///// Asegura que los endpoints de los controladores estén disponibles para el enrutamiento
app.Run();
