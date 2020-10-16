using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
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
        [InlineData(@"create", nameof(Contact.Name), nameof(Contact.Email), 
            nameof(Contact.Phone), nameof(Contact.Adress))]
        [InlineData(@"delete/3", "Contact3", "jenny@acme.com", "06-0003", "Atlantisplein 2 1000XX Amsterdam")]
        [InlineData(@"details/1", "Contact1", "mitch@acme.com", "06-0001", "polanenbuurt 1 1000BB Amsterdam")]
        [InlineData(@"edit/2", "Contact2", "peter@acme.com", "06-0002", "polanenbuurt 2 1000BB Amsterdam")]
        [InlineData(@"index", "mitch@acme.com", "Contact2", "06-0002", "Atlantisplein 2 1000XX Amsterdam")]
        public async Task Contact_Views_return_OKResult_and_contain_seeded_details(string view, params string[] expected)
        {
            var page = await _client.GetAsync("/contact/" + view);
            var content = await page.Content.ReadAsStringAsync();

            page.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainAll(expected);
        }

        [Theory]
        [InlineData(@"AddCourseDates/1", "")] //TODO add expected strings once coursdates are fully implemented
        [InlineData(@"create", nameof(Course.Name), nameof(Course.Description), nameof(Course.CourseDesign))]
        [InlineData(@"delete/3", "Course3", "complete course")]
        [InlineData(@"details/1", "Course1", "workshop", "CourseDesign1", "One weekend workshop")]
        [InlineData(@"edit/2", "Course2", "short course", "CourseDesign2")]
        [InlineData(@"index", "Course1", "short course", "CourseDesign3" ,"complete course",
             "Day2", "Edge")]
        public async Task Course_Views_return_OKResult_and_contain_seeded_details(string view, params string[] expected)
        {
            var page = await _client.GetAsync("/course/" + view);
            var content = await page.Content.ReadAsStringAsync();

            page.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainAll(expected);
        }
        
        //TODO add CourseDate Views tests once coursedates are fully implemented
        
        [Theory]
        [InlineData(@"create", nameof(CourseDesign.Name), nameof(CourseDesign.Description))]
        [InlineData(@"delete/3", "CourseDesign3", "Complete 3 seminars introduction course.")]
        [InlineData(@"details/1", "CourseDesign1", "One weekend workshop", "Seminar1", "Introduction seminar.",
            "Day1", "The modes Neutral and Overdrive.", "Overview")]
        [InlineData(@"edit/2", "CourseDesign2", "Short 4 day introduction course.", "Seminar3", "Day6", "Introduction")]
        [InlineData(@"index", "CourseDesign3", "One weekend workshop", "Seminar2", "Day5", "Edge")]
        public async Task CourseDesign_Views_return_OKResult_and_contain_seeded_details(
            string view, params string[] expected)
        {
            var page = await _client.GetAsync("/coursedesign/" + view);
            var content = await page.Content.ReadAsStringAsync();

            page.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainAll(expected);
        }
        
        [Theory]
        [InlineData(@"create", nameof(Day.Name), nameof(Day.Description))]
        [InlineData(@"delete/3", "Day3", "Deeper in the modes.")]
        [InlineData(@"details/1", "Day1", "Introduction day. Overview of the system, overall principles.", 
            "Overview", "What is cvt and how is it structured.", "CVT App pages 15-24")]
        [InlineData(@"edit/2", "Day2", "The modes Neutral and Overdrive.", "Neutral", 
            "Introduction", "Overview", "Support", "Overdrive")]
        [InlineData(@"index", "Day6", "All the modes.", "Overview", "Neutral")]
        public async Task Day_Views_return_OKResult_and_contain_seeded_details(string view, params string[] expected)
        {    
            var page = await _client.GetAsync("/day/" + view);
            var content = await page.Content.ReadAsStringAsync();

            page.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainAll(expected);
        }

        
    }
}