using CRUDPrograme.Filters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repository;
using RepositoryContracts;
using RepositoryImpl;
using Serilog;
using Serilog.Core;
using ServiceContracts;
using Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IPersonRespository, PersonRepository>();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

//builder.Services.AddControllersWithViews(options =>
//{
//    // 方式 A：泛型注册（无参过滤器或参数全由 DI 提供的场景）
//    // options.Filters.Add<PersonsListActionFilter>();

//    // 方式 B：手动实例化注册（需要传递自定义参数的场景）
//    // 通过临时构建的 ServiceProvider 解析构造所需的 ILogger
//    var serviceProvider = builder.Services.BuildServiceProvider();
//    var logger = serviceProvider.GetRequiredService<ILogger<ResponseHeaderAsyncActionFilter>>();

//    options.Filters.Add(new ResponseHeaderAsyncActionFilter("X-From-Global", "Global-Value", 3,logger));
//    //方式C
//    //options.Filters.Add(new TypeFilterAttribute(typeof(ResponseHeaderActionFilter))
//    //{
//    //    Order = 3,
//    //    Arguments = new object[] { "X-From-Global", "Global-Value" ,3}

//    //});
//});
//builder.Host.ConfigureLogging(logging =>
//{
//    logging.ClearProviders();
//    logging.AddConsole();
//    logging.AddDebug();
//});

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);

    loggerConfiguration.ReadFrom.Services(services);
});

builder.Services.AddHttpLogging(Options =>
{
    Options.LoggingFields = HttpLoggingFields.RequestProperties | 
    HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();
app.UseHttpLogging();
app.UseSerilogRequestLogging();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
ExcelPackage.License.SetNonCommercialPersonal("zank");
app.MapControllers();
app.UseRouting();
app.UseStaticFiles();
// 输出各级别日志
app.Run();

public partial class Program { }