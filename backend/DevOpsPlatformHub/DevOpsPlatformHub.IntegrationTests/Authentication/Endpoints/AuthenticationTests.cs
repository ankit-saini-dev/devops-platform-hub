using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Application.Dtos.Authentication;
using DevOpsPlatformHub.IntegrationTests.Authentication.Support;

namespace DevOpsPlatformHub.IntegrationTests.Authentication.Endpoints;

public sealed class AuthenticationTests : IClassFixture<AuthenticationWebApplicationFactory>
{
    private const string BaseRoute = "/api/Authentication";

    private readonly AuthenticationWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthenticationTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("https://localhost");
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ReturnsCreatedTokenAndCurrentUser()
    {
        // Arrange
        var username = CreateUsername();
        var request = CreateRegistrationRequest(username);

        try
        {
            // Act
            var registrationResponse = await _client.PostAsJsonAsync($"{BaseRoute}/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
            var authentication = await registrationResponse.Content.ReadFromJsonAsync<AuthenticationResult>();
            Assert.NotNull(authentication);
            Assert.NotEmpty(authentication.AccessToken);
            using var currentUserRequest = new HttpRequestMessage(HttpMethod.Get, $"{BaseRoute}/currentUser");
            currentUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.AccessToken);
            var currentUserResponse = await _client.SendAsync(currentUserRequest);
            Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);
            var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();
            Assert.NotNull(currentUser);
            Assert.Equal(request.Name, currentUser.Name);
            Assert.Equal(request.Username, currentUser.Username);
            Assert.Contains("User", currentUser.Roles);
        }
        finally
        {
            await _factory.DeleteUserByUsernameAsync(username);
        }
    }

    [Fact]
    public async Task LoginAsync_WithUsernameAndEmail_ReturnsAccessToken()
    {
        // Arrange
        var username = CreateUsername();
        var request = CreateRegistrationRequest(username) with
        {
            Email = $"{username.ToUpperInvariant()}@example.com"
        };

        try
        {
            var registrationResponse = await _client.PostAsJsonAsync($"{BaseRoute}/register", request);
            registrationResponse.EnsureSuccessStatusCode();
            var usernameLoginResponse = await _client.PostAsJsonAsync($"{BaseRoute}/login",
                new LoginRequest(request.Username, request.Password));
            var emailLoginResponse = await _client.PostAsJsonAsync($"{BaseRoute}/login",
                new LoginRequest(request.Email.ToLowerInvariant(), request.Password));

            // Act
            var usernameAuthentication = await usernameLoginResponse.Content.ReadFromJsonAsync<AuthenticationResult>();
            var emailAuthentication = await emailLoginResponse.Content.ReadFromJsonAsync<AuthenticationResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, usernameLoginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, emailLoginResponse.StatusCode);
            Assert.NotNull(usernameAuthentication);
            Assert.NotNull(emailAuthentication);
            Assert.NotEmpty(usernameAuthentication.AccessToken);
            Assert.NotEmpty(emailAuthentication.AccessToken);
        }
        finally
        {
            await _factory.DeleteUserByUsernameAsync(username);
        }
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ReturnsConflict()
    {
        // Arrange
        var username = CreateUsername();
        var request = CreateRegistrationRequest(username);

        try
        {
            var firstResponse = await _client.PostAsJsonAsync($"{BaseRoute}/register", request);
            firstResponse.EnsureSuccessStatusCode();
            var duplicateRequest = request with
            {
                Email = $"duplicate-{Guid.NewGuid():N}@example.com"
            };

            // Act
            var duplicateResponse = await _client.PostAsJsonAsync($"{BaseRoute}/register", duplicateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
        }
        finally
        {
            await _factory.DeleteUserByUsernameAsync(username);
        }
    }

    [Fact]
    public async Task CurrentUserAsync_WithoutBearerToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync($"{BaseRoute}/currentUser");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static string CreateUsername()
    {
        return $"integration-{Guid.NewGuid():N}";
    }

    private static RegistrationRequest CreateRegistrationRequest(string username)
    {
        return new RegistrationRequest(
            "Integration Test User",
            username,
            $"{username}@example.com",
            "A-strong-password-123");
    }
}
