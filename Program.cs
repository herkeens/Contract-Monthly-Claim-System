using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and SQL Server with Windows Authentication
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure session services (for storing user sessions such as role and user ID)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout setting
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
           .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ClaimValidator>());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session before authentication middleware
app.UseAuthorization();
app.Use(async (context, next) =>
{
    var session = context.Session;
    var isAuthenticated = session.GetString("UserRole") != null;

    if (!isAuthenticated && !context.Request.Path.StartsWithSegments("/Home/Dashboard")&&
        !context.Request.Path.StartsWithSegments("/Account/Login") &&
        !context.Request.Path.StartsWithSegments("/Account/Register") )
       
    {
        context.Response.Redirect("/Account/Login");
    }
    else
    {
        await next();
    }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.Run();
