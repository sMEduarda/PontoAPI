using Microsoft.EntityFrameworkCore;
using PontoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Serviços básicos
builder.Services.AddControllers();
builder.Services.AddDbContext<PontoContext>(opt =>
    opt.UseInMemoryDatabase("PontoDb"));

// OpenAPI nativo + Swagger UI
builder.Services.AddOpenApi();                    // Gera o documento OpenAPI
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configuração para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                             // Disponibiliza o /openapi/v1.json

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Ponto API v1");
        options.RoutePrefix = string.Empty;       // Abre o Swagger direto na raiz (/)
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();