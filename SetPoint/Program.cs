using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SetPoint.BLL._0.Common;
using SetPoint.BLL._0.Security;
using SetPoint.BLL._0.Sync;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.BLL._02.UsersInvitationManagement;
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
using SetPoint.DAL._2.Context;
using System.Text;
using System.Threading.RateLimiting;
//---------------------------------------------------------------------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);
//------------------------------------------------------------------------------------------ Configure Serilog logging ------------------------
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));
//------------------------------------------------------------------------------------------ Add services to the container. -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//------------------------------------------------------------------------------------------ Add CORS policy ----------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Allows any origin (such as your mobile IP address)
      .AllowAnyMethod()           // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
      .AllowAnyHeader();          // Allows any request header (tokens, content-type, etc.)
    });

    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
//------------------------------------------------------------------------------------------ Configure Database Context ----------------------
builder.Services.AddDbContext<SetPointDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));
//------------------------------------------------------------------------------------------ Configure AutoMapper -----------------------------
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
}).CreateMapper());
//------------------------------------------------------------------------------------------ Dependency Injection for BLL and Services --------
builder.Services.AddScoped<IUserBll, UserBll>();
builder.Services.AddScoped<IRoutineBll, RoutineBll>();
builder.Services.AddScoped<IExercisesBll, ExercisesBll>();
builder.Services.AddScoped<IMuscleGroupBll, MuscleGroupBll>();
builder.Services.AddScoped<IExerciseSetsBll, ExerciseSetsBll>();
builder.Services.AddScoped<IUserRelationBll, UserRelationBll>();
builder.Services.AddScoped<IRoutineRequestBll, RoutineRequestBll>();
builder.Services.AddScoped<IUsersInvitationBll, UsersInvitationBll>();
builder.Services.AddScoped<IWorkoutSessionsBll, WorkoutSessionsBll>();
builder.Services.AddScoped<IRoutineExercisesBll, RoutineExercisesBll>();
builder.Services.AddScoped<IBodyMeasurementsBll, BodyMeasurementsBll>();
builder.Services.AddScoped<IWorkoutExercisesBll, WorkoutExercisesBll>();
builder.Services.AddScoped<IExerciseMuscleGroupBll, ExerciseMuscleGroupBll>();
//------------------------------------------------------------------------------------------ Dependency Injection Services --------------------
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
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
var app = builder.Build();
app.UseForwardedHeaders();
//------------------------------------------------------------------------------------------- Configure Serilog request logging and global exception handling 
app.UseSerilogRequestLogging();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var ex = exceptionHandlerFeature?.Error;

        Log.Error(ex, "Unhandled exception in {Path}", exceptionHandlerFeature?.Path);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new SetPoint.API.Common.ApiResponse
        {
            WithError = true,
            Message = "Internal server error",
            StatusCode = 500
        });
    });
});
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
app.UseRouting();        // Ensures routing executes before authentication and authorization
app.UseRateLimiter();    // Ensures rate limiting executes before authorization to protect endpoints
app.UseAuthentication(); // Ensures authentication executes before authorization
app.UseAuthorization();  // Ensures authorization executes after authentication and rate limiting
app.MapControllers();    // Ensures controller endpoints are mapped and available for routing
app.Run();
