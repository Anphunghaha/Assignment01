using DataAccess.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repository.Implement;
using Repository.Interface;
using Service.DTO;
using Service.Implement;
using Service.Interface;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======== Add Services ========
builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel());
    })
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // JWT Auth in Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FUNews API", Version = "v1" });
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Enter JWT Bearer token only",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// JWT Auth config
// Lấy thông tin JWT từ appsettings
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});


builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("Admin");
    });
    c.AddPolicy("RequireStaffRole", policy =>
    {
        policy.RequireRole("1");
    });
});

// Admin Config
builder.Services.Configure<AdminConfiguration>(builder.Configuration.GetSection("AdminAccount"));

// Dependency Injection
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

// ======== Middleware ========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// ======== EDM model for OData ========
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    var newsArticle = builder.EntitySet<NewsArticle>("NewsArticles").EntityType;
    //newsArticle.HasKey(n => n.NewsArticleID);

    var category = builder.EntitySet<CategoryViewModel>("Categories").EntityType;
    category.HasKey(c => c.CategoryID); // hoặc tên thuộc tính khóa của CategoryViewModel

    return builder.GetEdmModel();
}
