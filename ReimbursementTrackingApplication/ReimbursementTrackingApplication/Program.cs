using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Misc;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;
using System.Text;

namespace ReimbursementTrackingApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddScoped<CustomExceptionFilter>();

            #region Context
            builder.Services.AddDbContext<ContextApp>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion

            #region mapper
            builder.Services.AddAutoMapper(typeof(User));
            builder.Services.AddAutoMapper(typeof(ReimbursementItem));
            builder.Services.AddAutoMapper(typeof(ReimbursementRequest));
            builder.Services.AddAutoMapper(typeof(ApprovalStage));
            builder.Services.AddAutoMapper(typeof(Employee));
            builder.Services.AddAutoMapper(typeof(ExpenseCategory));
            builder.Services.AddAutoMapper(typeof(Policy));
            builder.Services.AddAutoMapper(typeof(BankAccount));
            #endregion

            #region Repositories
            builder.Services.AddScoped<IRepository<int,User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int,ReimbursementRequest>, ReimbursementRequestRepository>();    
            builder.Services.AddScoped<IRepository<int,ReimbursementItem>,ReimbursementItemRepositories>();
            builder.Services.AddScoped<IRepository<int,ApprovalStage>, ApprovalStageRepository>();
            builder.Services.AddScoped<IRepository<int,BankAccount>,BankAccountRepository>();
            builder.Services.AddScoped<IRepository<int,Employee>,EmployeeRepository>();
            builder.Services.AddScoped<IRepository<int,ExpenseCategory>,CategoryRepositories>();
            builder.Services.AddScoped<IRepository<int,Policy>,PolicyRepository>();
            builder.Services.AddScoped<IRepository<int,Payment>,PaymentRepository>();

            #endregion
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "File");

            #region  Services
            builder.Services.AddScoped<IUserServices, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IReimbursementItemService>(
                provider =>
                    new ReimbursementItemService(
                        provider.GetRequiredService<IRepository<int, ReimbursementItem>>(),
                        provider.GetRequiredService<IRepository<int, ExpenseCategory>>(),
                        uploadFolder,
                        provider.GetRequiredService<IMapper>()
                        )
                );
            builder.Services.AddScoped<IReimbursementRequestService>(
                provider =>
                new ReimbursementRequestService(
                    provider.GetRequiredService<IRepository<int, ReimbursementRequest>>(),
                    provider.GetRequiredService<IRepository<int, Policy>>(),
                    provider.GetRequiredService<IMapper>(),
                    uploadFolder,
                    provider.GetRequiredService<IRepository<int, ReimbursementItem>>(),
                    provider.GetRequiredService<IRepository<int, Employee>>(),
                    provider.GetRequiredService<IRepository<int, ExpenseCategory>>(),
                    provider.GetRequiredService<IRepository<int, User>>()
                )
            );
            //builder.Services.AddScoped<IReimbursementRequestService, ReimbursementRequestService>();
            builder.Services.AddScoped<IPolicyService, PolicyService>();
            builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IApprovalService, ApprovalService>();
            builder.Services.AddScoped<IBankService, BankService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            #endregion

            #region Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                    };
                });
            #endregion


            builder.Services.AddControllers();

            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
         
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
