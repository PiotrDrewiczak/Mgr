using PolyGenerator;
using PolyGenerator.Interfaces;
using PolyGenerator.Scripts;
using WebApi.CP.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettingsModel>(
    builder.Configuration.GetSection(AppSettingsModel.ApplicationSettings));

builder.Services.AddScoped<ITriangulation, TrangulationGenerator>();
builder.Services.AddScoped<IQuadrangulation, QuadrangulationGenerator>();
builder.Services.AddScoped<IExcelGenerator, ExcelGenerator>();
builder.Services.AddScoped<IRectangleGenerator, RectangleGenerator>();
builder.Services.AddScoped<IPolygonGenerator, PolygonGenerator>(); 
builder.Services.AddScoped<IPrediction, PredictionGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

