using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;
using OfficeOpenXml;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
ExcelPackage.License.SetNonCommercialPersonal("zank");
app.MapControllers();
app.UseRouting();
app.UseStaticFiles();

app.Run();
