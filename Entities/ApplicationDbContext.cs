using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 3. 声明数据表映射集（DbSet 必须为 virtual 以便后续单元测试进行 Mock）
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*1.映射到表中 2.IO流读取json文件 3.JSON序列化为对象 4.通过HASDATA 将数据传入            */

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // JSON 属性名与 C# 属性大小写不一致（如 CountryID vs CountryId），需忽略大小写匹配
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            string contries_str = System.IO.File.ReadAllText("countries.json");

            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(contries_str, jsonOptions);

            if (countries !=null)
            {
                foreach (var country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
                
            }

            string persons_str = System.IO.File.ReadAllText("persons.json");

            List<Person>?  pesons= JsonSerializer.Deserialize<List<Person>>(persons_str, jsonOptions);

            if (pesons != null)
            {
                foreach (var person in pesons)
                {
                    modelBuilder.Entity<Person>().HasData(person);
                }

            }
        }
        public virtual List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXEC dbo.GetAllPersons").ToList();
        }
    }
}
