
using CoreMasterDetails.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ArtistDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("ArtistCon")));


var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "Default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();