using Entities;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUDTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                //获取到使用SQL的DB上下文，然后删除
                var dbOnSql= services.SingleOrDefault(d=>d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbOnSql != null)
                {
                    services.Remove(dbOnSql);
                }
                //设置DB上下文使用内存版本
                services.AddDbContext<ApplicationDbContext>(Options=>Options.UseInMemoryDatabase("testTable"));
                
            });
        }
    }
}
