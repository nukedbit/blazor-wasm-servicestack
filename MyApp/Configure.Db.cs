using MyApp.ServiceModel;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

[assembly: HostingStartup(typeof(MyApp.ConfigureDb))]

namespace MyApp
{
    // Example Data Model
    //public class MyTable
    //{
    //    [AutoIncrement]
    //    public int Id { get; set; }
    //    public string? Name { get; set; }
    //}

    public class ConfigureDb : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices((context,services) => services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                context.Configuration.GetConnectionString("DefaultConnection") ?? ":memory:",
                SqliteDialect.Provider)))
            .ConfigureAppHost(appHost =>
            {
                // Create non-existing Table and add Seed Data Example
                using var db = appHost.Resolve<IDbConnectionFactory>().Open();                
                if (db.CreateTableIfNotExists<Bookings>())
                {
                    // db.Insert(new MyTable { Name = "Seed Data for new MyTable" });
                }
            });
    }
}