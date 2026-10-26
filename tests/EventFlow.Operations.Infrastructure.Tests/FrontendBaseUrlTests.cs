using EventFlow.Operations.Infrastructure.Background;

namespace EventFlow.Operations.Infrastructure.Tests;

public sealed class FrontendBaseUrlTests
{
    [Theory]
    [InlineData("https://eventflow-web.duckdns.org", false, "https://eventflow-web.duckdns.org")]
    [InlineData("https://eventflow-web.duckdns.org/", false, "https://eventflow-web.duckdns.org")]
    [InlineData("http://localhost:4200", true, "http://localhost:4200")]
    public void Accepts_valid_urls(string configured, bool isDevelopment, string expected)
    {
        var result = EventInvitationCreatedConsumer.ResolveFrontendBaseUrl(configured, isDevelopment);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://eventflow-web.duckdns.org")]
    public void Rejects_invalid_urls(string? configured)
    {
        Assert.Throws<InvalidOperationException>(
            () => EventInvitationCreatedConsumer.ResolveFrontendBaseUrl(configured, isDevelopment: false));
    }

    [Theory]
    [InlineData("http://eventflow-web.duckdns.org")]
    [InlineData("http://localhost:4200")]
    [InlineData("https://localhost:4200")]
    [InlineData("https://127.0.0.1:4200")]
    public void Rejects_non_public_or_non_https_urls_outside_development(string configured)
    {
        Assert.Throws<InvalidOperationException>(
            () => EventInvitationCreatedConsumer.ResolveFrontendBaseUrl(configured, isDevelopment: false));
    }
}
