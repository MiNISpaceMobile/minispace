using Api;
using Api.Auth;
using Domain.Abstractions;
using Domain.Services.Abstractions;
using Domain.Services.Implementations;
using Domain.Services;
using Infrastructure.Authenticators;
using Infrastructure.CryptographyProviders;
using Infrastructure.DatabaseContexts;
using Infrastructure.JwtHandlers;
using Infrastructure.PingResponders;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(Path.Join(Directory.GetCurrentDirectory(), "../../minispace-secrets.json"), true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddJwtAuthorization().AddDefaultValues().EnableAnnotations());

builder.Services.AddControllers(options => options.Filters.Add(new ProducesAttribute("application/json")))
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

/* Add things to dependency injection below!
 * 
 * Most of the time use 'AddScoped'.
 * It means a new object will be created each time a controller method is run.
 * Sometimes you will have to use 'AddTransient'.
 * It means a new object will be created every time
 * you use this dependency EVEN within one controller method call.
 * And sometimes you may have to use 'AddSingleton'.
 * It means that only one object of your class will exist for entire program duration.
 */

// EF Core
builder.Services.AddEFContext<SqliteDbContext>();
builder.Services.AddScoped<IUnitOfWork, DatabaseUnitOfWork>();
// Auth:
builder.Services.AddSingleton<ICryptographyProvider<RSAParameters>, RsaConfigCryptographyProvider>();
builder.Services.AddScoped<IJwtHandler, MinispaceSignedJwtHandler>();
builder.Services.AddScoped<IAuthenticator, UsosAuthenticator>();
// Services:
builder.Services.AddSingleton<IPingResponder, PongPingResponder>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();

/* Warning! Important! Will help you later!
 * 
 * Try commenting the line with IPingResponder and then try executing 'ping' via swagger.
 * The app will compile, it will even run, but when you try 'ping'...
 * You will get an ugly Error 500 - Internal Server Error.
 * This is because the app tries to run 'Ping' method in the cotroller,
 * but can't satisfy all dependencies of the controller - namely IPingResponder.
 * So when you see this error it may mean you forgot to register your implementation here
 * ... or that you fucked up and got some other exception :)
 */

builder.Services.AddAuthentication(nameof(JwtAuthScheme))
                .AddScheme<JwtAuthScheme.Options, JwtAuthScheme.Handler>(nameof(JwtAuthScheme), null);

builder.Services.AddExceptionHandler<MinispaceExceptionHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(o => { });

app.MapControllers();

// Our own function that setups a few things
app.PerformCustomStartupActions(resetDb: true, generateDevAdminJwt: true);

app.Run();
