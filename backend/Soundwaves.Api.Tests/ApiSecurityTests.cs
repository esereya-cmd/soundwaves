using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Soundwaves.Api.Tests;

public class ApiSecurityTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiSecurityTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder => builder.UseEnvironment("Testing"))
            .CreateClient();
    }

    [Fact]
    public async Task TrackId_Zero_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/tracks/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TrackId_Unknown_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/tracks/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/tracks/-1")]
    [InlineData("/api/tracks/abc")]
    [InlineData("/api/tracks/18446744073709551616")]
    public async Task TrackId_InvalidFormat_ReturnsClientError(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.True(
            (int)response.StatusCode >= 400 &&
            (int)response.StatusCode < 500,
            $"Expected 4xx but got {(int)response.StatusCode} for {url}");
    }

    [Theory]
    [InlineData("/api/tracks/0")]
    [InlineData("/api/tracks/999999")]
    [InlineData("/api/tracks/abc")]
    public async Task InvalidTrackRequest_DoesNotExposeInternalInformation(string url)
    {
        var response = await _client.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        Assert.True((int)response.StatusCode >= 400);

        Assert.DoesNotContain("System.Exception", body);
        Assert.DoesNotContain("StackTrace", body);
        Assert.DoesNotContain("C:\\", body);
        Assert.DoesNotContain("/app/", body);
        Assert.DoesNotContain("ConnectionString", body);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
    }}

