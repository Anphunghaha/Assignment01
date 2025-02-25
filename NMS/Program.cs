using DataAccess.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Repository.Implement;
using Repository.Interface;
using Service.DTO;
using Service.Implement;
using Service.Interface;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<FunewsManagementContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.Configure<AdminConfiguration>(builder.Configuration.GetSection("AdminAccount"));

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Account/Login";
           options.LogoutPath = "/Account/Logout";
           options.AccessDeniedPath = "/Account/AccessDenied";
           options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
       });

        // Register dependencies
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
        builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IReportService, ReportService>();
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
