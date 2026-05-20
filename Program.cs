using CNPM_DeMo.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LibManageDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Chưa đăng nhập thì đá về đây
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Đăng nhập sai quyền thì đá về đây <--- THÊM DÒNG NÀY
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
var cultureInfo = new CultureInfo("vi-VN");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
});


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

app.UseAuthentication(); // BẮT BUỘC PHẢI THÊM DÒNG NÀY TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
