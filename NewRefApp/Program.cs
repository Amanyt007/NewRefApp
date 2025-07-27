using Microsoft.EntityFrameworkCore;
using NewRefApp.Controllers;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));




builder.Services.AddScoped<UserFilter>();
builder.Services.AddScoped<AdminFilter>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUpiDetailsService, UpiDetailsService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IBankDetailsService, BankDetailsService>();
builder.Services.AddScoped<ITransactionDetailsService, TransactionDetailsService>();
builder.Services.AddScoped<IInvestmentPlanService, InvestmentPlanService>();
builder.Services.AddScoped<IUserInvestmentService, UserInvestmentService>();
builder.Services.AddScoped<AdminController>();
builder.Services.AddScoped<IWithdrawService, WithdrawService>();
builder.Services.AddScoped<IReferralProgramService, ReferralProgramService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
//builder.Services.AddScoped<IInvestmentPlanService, InvestmentPlanService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
