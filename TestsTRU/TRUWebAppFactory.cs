using LibraryTRU.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace TestsTRU
{
    public class TRUWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        public TRUWebAppFactory()
        {
            var backupFile = Directory.GetFiles("../../..", "*.sql", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("P@ssword1")
                .WithBindMount(backupFile.FullName, "/docker-entrypoint-initdb.d/init.sql")
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services => {
                services.RemoveAll(typeof(DbContextOptions<PostgresContext>));

                services.AddDbContext<PostgresContext>(o =>
                {
                    o.UseNpgsql(_dbContainer.GetConnectionString());
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }
    }
}
