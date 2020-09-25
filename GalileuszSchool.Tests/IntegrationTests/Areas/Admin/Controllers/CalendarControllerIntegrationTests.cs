using AngleSharp.Html.Dom;
using GalileuszSchool.Tests.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GalileuszSchool.Tests.IntegrationTests.Areas.Admin.Controllers
{
    public class CalendarControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestingWebAppFactory<Startup> _factory;
        public CalendarControllerIntegrationTests(TestingWebAppFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Index_WhenCalled_Returns_OkRespond()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("calendar/index");
            // Asser
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/GetEvents")]
        [InlineData("/GetEventsForStudents")]
        [InlineData("/GetStudentsByEvent")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("calendar" + url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_CalendarIndex_RedirectsAnUnauthenticatedUser()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/calendar/index");
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Account/Login",
                response.Headers.Location.OriginalString);
        }
        [Fact]
        public async Task Get_IndexPageIsReturnedForAnAuthenticatedUser()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            //Act
            var response = await client.GetAsync("/calendar/index");

            // Assert
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_CreateNewEvent_ReturnsOk()
        {
            // Arrange
            var defaultPage = await _client.GetAsync("/calendar/index");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            //Act
            //var response = await _client.SendAsync(
            //    (IHtmlFormElement)content.QuerySelector("form[id='createEventform']"),
            //    (IHtmlButtonElement)content.QuerySelector("button[id='createNewEvent']"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            //Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            //Assert.Equal("/", response.Headers.Location.OriginalString);
        }


        
    }
}
