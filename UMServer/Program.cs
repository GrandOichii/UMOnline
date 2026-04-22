using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;
using UMServer.Hubs;
using UMServer.Repositories;
using UMServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UMContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("UMContext"))
);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// DB Contexts

builder.Services.AddTransient<UMContext>();

// Repositories
builder.Services.AddSingleton<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
builder.Services.AddTransient<ILoadoutRepository, LoadoutRepository>();
builder.Services.AddTransient<IMatchConfigRepository, MatchConfigRepository>();
builder.Services.AddTransient<ICoreScriptRepository, CoreScriptRepository>();
builder.Services.AddTransient<IContentUpdateRepository, ContentUpdateRepository>();

// Business logic
builder.Services.AddTransient<ILoadoutManager, LoadoutManager>();
builder.Services.AddTransient<ICoreScriptManager, CoreScriptManager>();
builder.Services.AddTransient<IUpdateManager, UpdateManager>();
builder.Services.AddTransient<IMatchManager, MatchManager>();
builder.Services.AddTransient<IMatchConfigManager, MatchConfigManager>();

// Services
builder.Services.AddSingleton<IMatchConnectEndpointSerializer, MatchConnectEndpointSerializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// if (app.Environment.IsProduction())
// {
//     using var scope = app.Services.CreateScope();
//     var ctx = scope.ServiceProvider.GetRequiredService<UMContext>();
//     await ctx.Database.MigrateAsync();
// }

app.UseHttpsRedirection();

app.MapControllers();
app.UseWebSockets();
app.MapHub<MatchesHub>("/Matches");

app.Run();
