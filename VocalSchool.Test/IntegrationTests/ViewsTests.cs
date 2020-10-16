using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using VocalSchool.Models;
using VocalSchool.Test.Infrastructure;
using Xunit;

namespace VocalSchool.Test.IntegrationTests
{
    public class ViewsTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public ViewsTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData(@"index", "mitch@acme.com", "Contact2", "06-0002", "Atlantisplein 2 1000XX Amsterdam")]
        [InlineData(@"details/1", "Contact1", "mitch@acme.com", "06-0001", "polanenbuurt 1 1000BB Amsterdam")]
        [InlineData(@"edit/2", "Contact2", "peter@acme.com", "06-0002", "polanenbuurt 2 1000BB Amsterdam")]
        [InlineData(@"delete/3", "Contact3", "jenny@acme.com", "06-0003", "Atlantisplein 2 1000XX Amsterdam")]
        [InlineData(@"create", nameof(Contact.Name), nameof(Contact.Email), 
            nameof(Contact.Phone), nameof(Contact.Adress))]
        public async Task Contact_Views_return_OKResult_and_contain_seeded_details(string view, params string[] expected)
        {
            var page = await _client.GetAsync("/contact/" + view);
            var content = await page.Content.ReadAsStringAsync();

            page.StatusCode.Should().Equals(HttpStatusCode.OK);
            content.Should().ContainAll(expected);
        }

    }
}