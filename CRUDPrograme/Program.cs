using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});
var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();
app.UseRouting();
app.UseStaticFiles();

app.Run();
