using System;
using System.Net.Http;
using System.Threading.Tasks;
using AuthDemo.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace AuthDemo.Tests
{
    public class AuthTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AuthTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetWeatherForecast_Authorize_ShouldSucceed()
        {
            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
