using Microsoft.Extensions.DependencyInjection;

namespace SignalRHubDocs.Tests;

public class HubInspectorServiceTests
{
    [Fact]
    public void InspectHub_BasicHub_ReturnsCorrectHubInfo()
    {
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);

        var hubType = typeof(TestHub);
        var route = "/testHub";
        var result = hubInspector.InspectHub(hubType, route);
        Assert.Equal("Test", result.Name);
        Assert.Equal("/testHub", result.Route);
        Assert.False(result.RequiresAuth);
        Assert.Equal(3, result.Methods.Count);
        Assert.Equal("TestMethod", result.Methods[0].Name);
    }

    [Fact]
    public void InspectHub_HubWithDocumentationAttribute_UsesAttributeValues()
    {
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);

        var hubType = typeof(DocumentedHub);
        var route = "/documented";
        var result = hubInspector.InspectHub(hubType, route);
        Assert.Equal("Custom Hub Name", result.Name);
        Assert.Equal("Custom hub description", result.Description);
    }

    [Fact]
    public void InspectHub_AuthorizedHub_ReturnsRequiresAuth()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);

        var hubType = typeof(AuthorizedHub);
        var route = "/authorized";
        var result = hubInspector.InspectHub(hubType, route);
        Assert.True(result.RequiresAuth);
    }

    [Fact]
    public void InspectHub_MethodWithParameters_ReturnsCorrectParameters()
    {
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);

        var hubType = typeof(TestHub);
        var route = "/testHub";
        var result = hubInspector.InspectHub(hubType, route);
        var methodWithParams = result.Methods.FirstOrDefault(m => m.Name == "MethodWithParameters");
        Assert.NotNull(methodWithParams);
        Assert.Equal(2, methodWithParams.Parameters.Count);
        Assert.Equal("message", methodWithParams.Parameters[0].Name);
        Assert.Equal("String", methodWithParams.Parameters[0].Type);
        Assert.Equal("count", methodWithParams.Parameters[1].Name);
        Assert.Equal("Int32", methodWithParams.Parameters[1].Type);
    }

    [Fact]
    public void InspectHub_StreamingMethod_ReturnsCorrectReturnType()
    {
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);

        var hubType = typeof(TestHub);
        var route = "/testHub";
        var result = hubInspector.InspectHub(hubType, route);
        var streamingMethod = result.Methods.FirstOrDefault(m => m.Name == "StreamingMethod");
        Assert.NotNull(streamingMethod);
        Assert.Equal("String", streamingMethod.ReturnType);
        Assert.NotNull(streamingMethod.ReturnSchema);
    }

    [Fact]
    public void InspectHub_AuthorizedMethod_ReturnsAuthRequirements()
    {
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);
        var hubType = typeof(AuthorizedHub);
        var route = "/authorized";
        var result = hubInspector.InspectHub(hubType, route);
        var adminMethod = result.Methods.FirstOrDefault(m => m.Name == "AdminMethod");
        Assert.NotNull(adminMethod);
        Assert.True(adminMethod.RequiresAuth);
        Assert.Contains("Admin", adminMethod.RequiredRoles!);
    }

    [Fact]
    public void InspectHub_FrameworkMethods_AreExcluded()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();
        var hubInspector = new HubInspectorService(serviceProvider);
        var hubType = typeof(HubWithFrameworkMethods);
        var route = "/framework";
        var result = hubInspector.InspectHub(hubType, route);
        Assert.Single(result.Methods);
        Assert.Equal("UserMethod", result.Methods[0].Name);
    }
}