using DotNetEnv;
using E_Commerce.Api.Data;
using E_Commerce.Api.Repository;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            Env.Load();

            var DefualtConnection = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            option.UseSqlServer(DefualtConnection));
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddScoped<IEmailService, EmailServer>();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

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