namespace SignalRHubDocs.Tests;

public class EmbeddedTemplateServiceTests
{
    private readonly EmbeddedTemplateService _templateService;

    public EmbeddedTemplateServiceTests()
    {
        _templateService = new EmbeddedTemplateService();
    }

    [Fact]
    public void ProcessTemplate_SimpleReplacement_ReplacesCorrectly()
    {
        var template = "Hello {{Name}}, welcome to {{App}}!";
        var replacements = new Dictionary<string, string>
        {
            { "Name", "John" },
            { "App", "SignalR Docs" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal("Hello John, welcome to SignalR Docs!", result);
    }

    [Fact]
    public void ProcessTemplate_NoReplacements_ReturnsOriginalTemplate()
    {
        var template = "Static content without placeholders";
        var replacements = new Dictionary<string, string>();
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal(template, result);
    }

    [Fact]
    public void ProcessTemplate_MissingPlaceholder_LeavesPlaceholderIntact()
    {
        var template = "Hello {{Name}}, your {{Status}} is {{Unknown}}";
        var replacements = new Dictionary<string, string>
        {
            { "Name", "Alice" },
            { "Status", "Active" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal("Hello Alice, your Active is {{Unknown}}", result);
    }

    [Fact]
    public void ProcessTemplate_MultipleOccurrences_ReplacesAll()
    {
        var template = "{{Title}} - {{Title}} Documentation for {{Title}}";
        var replacements = new Dictionary<string, string>
        {
            { "Title", "SignalR" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal("SignalR - SignalR Documentation for SignalR", result);
    }

    [Fact]
    public void ProcessTemplate_HtmlContent_PreservesHtmlStructure()
    {
        var template = "<html><head><title>{{Title}}</title></head><body>{{Content}}</body></html>";
        var replacements = new Dictionary<string, string>
        {
            { "Title", "Test Page" },
            { "Content", "<h1>Welcome</h1><p>Test content</p>" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Contains("<title>Test Page</title>", result);
        Assert.Contains("<h1>Welcome</h1><p>Test content</p>", result);
    }

    [Fact]
    public void ProcessTemplate_UrlReplacements_HandlesUrlsCorrectly()
    {
        var template = "<script src=\"{{SignalRClientUrl}}\"></script><script src=\"{{MessagePackClientUrl}}\"></script>";
        var replacements = new Dictionary<string, string>
        {
            { "SignalRClientUrl", "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js" },
            { "MessagePackClientUrl", "https://cdn.jsdelivr.net/npm/@microsoft/signalr-protocol-msgpack@8.0.0/dist/browser/signalr-protocol-msgpack.min.js" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Contains("microsoft-signalr/8.0.0/signalr.min.js", result);
        Assert.Contains("signalr-protocol-msgpack@8.0.0", result);
    }

    [Fact]
    public void ProcessTemplate_SpecialCharacters_HandlesCorrectly()
    {
        var template = "Message: {{Message}}";
        var replacements = new Dictionary<string, string>
        {
            { "Message", "Special chars: <>&\"'`" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal("Message: Special chars: <>&\"'`", result);
    }

    [Fact]
    public void ProcessTemplate_EmptyValues_ReplacesWithEmpty()
    {
        var template = "Before{{Empty}}After";
        var replacements = new Dictionary<string, string>
        {
            { "Empty", "" }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal("BeforeAfter", result);
    }

    [Theory]
    [InlineData("{{Key}}", "Value", "Value")]
    [InlineData("prefix{{Key}}suffix", "Value", "prefixValuesuffix")]
    [InlineData("{{Key1}}{{Key2}}", "A", "A{{Key2}}")]
    public void ProcessTemplate_VariousFormats_WorksCorrectly(string template, string value, string expected)
    {
        var replacements = new Dictionary<string, string>
        {
            { "Key", value },
            { "Key1", value }
        };
        var result = _templateService.ProcessTemplate(template, replacements);
        Assert.Equal(expected, result);
    }
}