using Azure.Storage.Blobs;
using Mango.Web.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using RobertTest.Data;
using RobertTest.Service;
using RobertTest.Services;
using RobertTest.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

/********** configure Identity Authentication ********************/
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
/******************************************************************/

SD.AuthApiBase = builder.Configuration["ServiceUrls:AuthApi"];

/************* Entity FrameWork Connect to Sql Server ****************************/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LOCAL"));
});
/*********************************************************************************/

/***************************** adding Blob Service to connect Azure Blob *********************************/

builder.Services.AddSingleton(u=> new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));
builder.Services.AddSingleton<IBlobService, BlobService>();

/*********************************************************************************************************/
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .AllowAnyHeader()
       
        );
});


var app = builder.Build();


Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings:SecretKey").Get<string>();
app.UseCors("AllowAll");



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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
