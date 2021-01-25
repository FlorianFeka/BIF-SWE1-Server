using FluentAssertions;
using SimpleServer.Implementations;
using System;
using Xunit;

namespace SimpleServer.Test.Implementations
{
    public class Url_Test
    {
        [Fact]
        public void UrlConstructor_NullUrl_ShouldThrowArgumentNullException()
        {
            Action act = () => new Url(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UrlConstructor_BlankUrl_ShouldParseCorrectly()
        {
            var rawUrl = "localhost/";
            Url url = new Url(rawUrl);
            url.RawUrl.Should().Be(rawUrl);
            url.Path.Should().Be("/");
            url.Segments.Length.Should().Be(0);
        }

        [Fact]
        public void UrlConstructor_UrlWithoutParametersAndFragment_ShouldParseCorrectly()
        {
            var rawUrl = "localhost/test/site";
            Url url = new Url(rawUrl);
            url.RawUrl.Should().Be(rawUrl);
            url.Path.Should().Be("/test/site");
            url.Segments.Length.Should().Be(2);
            url.Segments.Should().Contain("test");
            url.Segments.Should().Contain("site");
        }

        [Fact]
        public void UrlConstructor_UrlWithParametersButWithoutFragment_ShouldParseCorrectly()
        {
            var rawUrl = "localhost/test/site?foo=bar&test=rest";
            Url url = new Url(rawUrl);
            url.RawUrl.Should().Be(rawUrl);
            url.Path.Should().Be("/test/site");
            url.Segments.Length.Should().Be(2);
            url.Segments.Should().Contain("test");
            url.Segments.Should().Contain("site");
            url.Parameter.Should().ContainKeys("foo", "test");
            url.Parameter.Should().ContainValues("bar", "rest");
        }

        [Fact]
        public void UrlConstructor_UrlWithoutParametersButWithFragment_ShouldParseCorrectly()
        {
            var rawUrl = "localhost/test/site#crash";
            Url url = new Url(rawUrl);
            url.RawUrl.Should().Be(rawUrl);
            url.Path.Should().Be("/test/site");
            url.Segments.Length.Should().Be(2);
            url.Segments.Should().Contain("test");
            url.Segments.Should().Contain("site");
            url.Fragment.Should().Be("crash");
        }

        [Fact]
        public void UrlConstructor_UrlWithParametersAndFragment_ShouldParseCorrectly()
        {
            var rawUrl = "localhost/test/site?foo=bar&test=rest#crash";
            Url url = new Url(rawUrl);
            url.RawUrl.Should().Be(rawUrl);
            url.Path.Should().Be("/test/site");
            url.Segments.Length.Should().Be(2);
            url.Segments.Should().Contain("test");
            url.Segments.Should().Contain("site");
            url.Fragment.Should().Be("crash");
            url.Parameter.Should().ContainKeys("foo", "test");
            url.Parameter.Should().ContainValues("bar", "rest");
        }
    }
}
