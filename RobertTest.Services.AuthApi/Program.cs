using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RobertTest.Services.AuthApi.Data;
using RobertTest.Services.AuthApi.Models;
using RobertTest.Services.AuthApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/************ EF AppDbContext ************/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LOCAL"));
});
/*****************************************/

/**** config to access JwtOptions in appsettings   ****/
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
/******************************************************/

/************************ Identity with EF ************/
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();
/******************************************************/


builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();


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

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}