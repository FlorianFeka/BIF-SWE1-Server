using FluentAssertions;
using SimpleServer.Http;
using SimpleServer.Implementations;
using Xunit;

namespace SimpleServer.Test.Implementations
{
    public class HttpResponse_Test
    {
        [Fact]
        public void SetStatus_ImATeapot_ShouldParseCorrectly()
        {
            var statusString = "I'm a teapot";
            var statusEnum = HttpStatus.I___m_a_teapot;
            var response = new HttpResponse();
            response.SetStatus(statusEnum);
            response.Status.Should().Be(statusString);
        }
        [Fact]
        public void SetStatus_NonAuthoritativeInformation_ShouldParseCorrectly()
        {
            var statusString = "Non-authoritative Information";
            var statusEnum = HttpStatus.Non__authoritative_Information;
            var response = new HttpResponse();
            response.SetStatus(statusEnum);
            response.Status.Should().Be(statusString);
        }
    }
}
