using DevPulse.Api.Extentions;
using DevPulse.Api.Middleware;
using DevPulse.Infrastructure;
using DevPulse.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();



#region Rate Limiter Configuration for Login And Error Create Api
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    options.AddPolicy("ErrorCreatePolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 25,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
});

#endregion

#region Api Documentatioon Configurations 
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "DevPulse Services";
        document.Info.Version = "v1";

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            };
        return Task.CompletedTask;
    });
});
#endregion

#region Add Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
}).AddApiKeyScheme();
#endregion

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddApplicationLayer();

builder.Services.AddCors();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DevPulseDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(cf => cf.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
    app.MapScalarApiReference();
}

app.UseRouting();

app.UseRateLimiter();

app.UseExceptionHandler();

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


#region Header Security 

app.UseSecurityHeaders(
    new HeaderPolicyCollection().AddContentTypeOptionsNoSniff()
    .AddFrameOptionsSameOrigin()
    .AddContentSecurityPolicy(builder => {
        // به طور پیش‌فرض فقط اجازه لود منابع از دامنه خودی (همان دامنه) را بده
        builder.AddDefaultSrc().Self();

        // اجازه اجرای اسکریپت‌ها فقط از دامنه خودی
        builder.AddScriptSrc().Self();

        // اجازه لود استایل‌ها فقط از دامنه خودی 
        // (نکته: اگر از کتابخانه‌هایی مثل MUI یا AntD استفاده می‌کنید که استایل اینلاین تزریق می‌کنند، ممکن است نیاز به .WithUnsafeInline() داشته باشید)
        builder.AddStyleSrc().Self();

        // جلوگیری از لود فونت از منابع خارجی (اختیاری اما توصیه شده)
        builder.AddFontSrc().Self();

        // جلوگیری از لود تصاویر از منابع خارجی (اختیاری)
        builder.AddImgSrc().Self().Data(); // Data برای اجازه دادن به Base64 images

        // غیرفعال کردن کامل object, embed, frame (امنیت بالا)
        builder.AddObjectSrc().None();
        builder.AddFrameAncestors().None();
    })
    // هدرهای پیشنهادی اضافی برای امنیت بیشتر
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .AddPermissionsPolicy(builder =>
    {
        builder.AddCamera().None();
        builder.AddMicrophone().None();
        builder.AddGeolocation().None();
    }));
#endregion

app.Run();
