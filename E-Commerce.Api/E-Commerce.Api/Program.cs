using DotNetEnv;
using E_Commerce.Api.Data;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            Env.Load();

            var DefualtConnection = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");


            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            option.UseSqlServer(DefualtConnection));




            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
