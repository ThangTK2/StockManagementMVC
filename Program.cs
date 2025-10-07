using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementMVC.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm Identity Authentication (login, register)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                                       options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<DataContext>();

//Connect sql server
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  //xác thực trước xác minh danh tính
app.UseAuthorization();   //xác minh danh tính

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
