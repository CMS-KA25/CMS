using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using Xunit;

namespace CMS.Api.Tests
{
    public class PatientApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PatientApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with in-memory
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CmsDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<CmsDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
        }

        [Fact]
        public async Task GetPatients_ReturnsSuccess()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/Patient");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var json = await resp.Content.ReadAsStringAsync();
            // should be valid JSON
            JsonDocument.Parse(json);
        }
    }
}