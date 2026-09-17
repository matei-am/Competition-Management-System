using CompetitionManager.Application.Services;
using CompetitionManager.Domain.Repositories;
using CompetitionManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ICompetitionRepository, CompetitionRepository>();
builder.Services.AddSingleton<ICompetitionService, CompetitionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
