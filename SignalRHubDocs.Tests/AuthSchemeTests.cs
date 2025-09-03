namespace SignalRHubDocs.Tests;

public class AuthSchemeTests
{
    [Fact]
    public void AuthScheme_DefaultValues_AreCorrect()
    {
        var authScheme = new AuthScheme();
        Assert.Equal(string.Empty, authScheme.Name);
        Assert.Equal(AuthType.Bearer, authScheme.Type);
        Assert.Null(authScheme.HeaderName);
        Assert.Null(authScheme.QueryParamName);
        Assert.Null(authScheme.CookieName);
        Assert.Equal(string.Empty, authScheme.Description);
        Assert.False(authScheme.IsDefault);
    }

    [Theory]
    [InlineData(AuthType.Bearer)]
    [InlineData(AuthType.ApiKey)]
    [InlineData(AuthType.QueryParam)]
    [InlineData(AuthType.Cookie)]
    [InlineData(AuthType.CustomHeader)]
    public void AuthType_AllEnumValues_AreSupported(AuthType authType)
    {
        // Arrange & Act
        var authScheme = new AuthScheme { Type = authType };

        // Assert
        Assert.Equal(authType, authScheme.Type);
    }

    [Fact]
    public void AuthScheme_BearerConfiguration_IsCorrect()
    {
        var authScheme = new AuthScheme
        {
            Name = "Bearer",
            Type = AuthType.Bearer,
            Description = "JWT Bearer token authentication",
            IsDefault = true
        };
        Assert.Equal("Bearer", authScheme.Name);
        Assert.Equal(AuthType.Bearer, authScheme.Type);
        Assert.Equal("JWT Bearer token authentication", authScheme.Description);
        Assert.True(authScheme.IsDefault);
    }

    [Fact]
    public void AuthScheme_CustomHeaderConfiguration_IsCorrect()
    {
        var authScheme = new AuthScheme
        {
            Name = "ApiKey",
            Type = AuthType.CustomHeader,
            HeaderName = "X-API-Key",
            Description = "API key authentication"
        };

        Assert.Equal("ApiKey", authScheme.Name);
        Assert.Equal(AuthType.CustomHeader, authScheme.Type);
        Assert.Equal("X-API-Key", authScheme.HeaderName);
        Assert.Equal("API key authentication", authScheme.Description);
    }

    [Fact]
    public void AuthScheme_QueryParamConfiguration_IsCorrect()
    {
        var authScheme = new AuthScheme
        {
            Name = "AccessToken",
            Type = AuthType.QueryParam,
            QueryParamName = "access_token",
            Description = "Token via query parameter"
        };
        Assert.Equal("AccessToken", authScheme.Name);
        Assert.Equal(AuthType.QueryParam, authScheme.Type);
        Assert.Equal("access_token", authScheme.QueryParamName);
        Assert.Equal("Token via query parameter", authScheme.Description);
    }

    [Fact]
    public void AuthScheme_CookieConfiguration_IsCorrect()
    {
        var authScheme = new AuthScheme
        {
            Name = "SessionCookie",
            Type = AuthType.Cookie,
            CookieName = "auth_session",
            Description = "Session cookie authentication"
        };
        Assert.Equal("SessionCookie", authScheme.Name);
        Assert.Equal(AuthType.Cookie, authScheme.Type);
        Assert.Equal("auth_session", authScheme.CookieName);
        Assert.Equal("Session cookie authentication", authScheme.Description);
    }
}