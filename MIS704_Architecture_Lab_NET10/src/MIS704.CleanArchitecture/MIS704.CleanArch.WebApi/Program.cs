
using MIS704.CleanArch.Application.UseCases.CrearPedido;
using MIS704.CleanArch.Infrastructure.Persistence;
using MIS704.CleanArchitecture.Domain.Repositories;

namespace MIS704.CleanArch.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registro de dependencias respetando la regla de Clean Architecture
            builder.Services.AddScoped<IPedidoRepository, SqlPedidoRepository>();
            builder.Services.AddScoped<CrearPedidoHandler>();

            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
