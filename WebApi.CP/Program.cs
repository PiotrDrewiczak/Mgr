using WebApi.CP.Models;
using PolyGenerator.Interfaces;
using PolyGenerator;
using PolyGenerator.Scripts;

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

builder.Services.AddScoped<ITriangulation, Trangulation>();
builder.Services.AddScoped<IQuadrulation, Quadrulation>();
builder.Services.AddScoped<IRectangleGenerator, RectangleGenerator>();
builder.Services.AddScoped<IPolygonGenerator, PolygonGenerator>();

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
