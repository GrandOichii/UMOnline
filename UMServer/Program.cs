using UMModel;
using UMServer.BusinessLogic;
using UMServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Contexts

builder.Services.AddTransient<UMContext>();

// Repositories
builder.Services.AddTransient<ILoadoutRepository, LoadoutRepository>();
builder.Services.AddTransient<ICoreScriptRepository, CoreScriptRepository>();

// Business logic
builder.Services.AddTransient<ILoadoutManager, LoadoutManager>();
builder.Services.AddTransient<ICoreScriptManager, CoreScriptManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
