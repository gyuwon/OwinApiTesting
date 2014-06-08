using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContactManager.Tests
{
    [TestClass]
    public class ContactsApiTest
    {
        private TestServer _server;

        [TestInitialize]
        public void Initialize()
        {
            this._server = TestServer.Create<Startup>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._server.Dispose();
        }

        [TestMethod]
        public async Task Post_Should_Respond_BadRequest_IfModelInvalid()
        {
            // Arrange

            // Act
            var res = await this._server.HttpClient.PostAsJsonAsync("api/Contacts", new Contact());

            // Assert
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
