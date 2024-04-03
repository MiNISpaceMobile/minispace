using Domain.Abstractions;
using Domain.Services;
using Infrastructure.DatabaseContexts;
using Infrastructure.PingResponders;
using Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddDbContext<DbContext, SqliteDbContext>(EntityFrameworkConfiguration.Configure);

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

builder.Services.AddSingleton<IPingResponder, PongPingResponder>();

builder.Services.AddScoped<IUnitOfWork, DatabaseUnitOfWork>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

/* 3 lines of code below automatically create and/or update development database.
 * You can find it in a subfolder 'MinispaceDb' of app's working directory,
 * which defaults to Api project directory. If something (like migrations) breaks horribly,
 * you can always recreate from scratch it - just delete it.
 */
{
    using var scope = app.Services.CreateScope();
    using var sqlite = scope.ServiceProvider.GetService<SqliteDbContext>();
    sqlite?.CreateOrUpdate();
}

app.Run();
