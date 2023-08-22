using API.Extensions;
using API.Middlewares;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Extension method for adding services to make program file clean
builder.Services.AddApplicationServices(builder.Configuration);
//Extension method identity services
builder.Services.AddIdentityServices(builder.Configuration);
//Extension method for swagger documentation
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");


//Swagger settings
//if (app.Environment.IsDevelopment())
//{
app.UseSwaggerDocumentation();
//}
app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<StoreContext>();

//For Identity
var identityContext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
//Logger
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await context.Database.MigrateAsync();
    //Identity Db
    await identityContext.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context); // Seed product data
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);//seed user data
}
catch (Exception ex)
{
    logger.LogDebug(ex, "An error occurred during migration");
}

app.Run();
