using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRHubDocs;

public class HubInspectorService
{
    private readonly ConcurrentDictionary<Type, HubInfo> _hubCache = new();
    private readonly IServiceProvider _serviceProvider;

    public HubInspectorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<(Type HubType, string Route)> GetSignalRHubsAsync()
    {
        var endpointDataSource = _serviceProvider.GetRequiredService<EndpointDataSource>();

        var signalRHubs = endpointDataSource.Endpoints
            .Where(endpoint => endpoint.Metadata.GetMetadata<HubMetadata>() != null &&
            endpoint.Metadata.GetMetadata<NegotiateMetadata>() == null)
            .Select(endpoint =>
            {
                var hubMetadata = endpoint.Metadata.GetMetadata<HubMetadata>();
                var routePattern = endpoint.Metadata.GetMetadata<RouteEndpoint>()?.RoutePattern?.RawText
                    ?? endpoint.DisplayName;

                return (hubMetadata!.HubType, Route: routePattern ?? "Unknown");
            })
            .ToList();

        return signalRHubs;
    }

    public SignalRDocumentation GenerateDocumentation(Assembly assembly, bool sendEnumByString = false)
    {
        var documentation = new SignalRDocumentation();
        var hubTypes = GetSignalRHubsAsync();

        foreach ((Type hubType, string route) in hubTypes)
        {
            var hubInfo = InspectHub(hubType, route, sendEnumByString);
            documentation.Hubs.Add(hubInfo);
        }

        return documentation;
    }

    private bool IsFrameworkMethod(MethodInfo method)
    {
        var frameworkMethods = new[]
        {
            "OnConnectedAsync",
            "OnDisconnectedAsync",
            "Dispose",
            "DisposeAsync"
        };

        return frameworkMethods.Contains(method.Name);
    }

    public HubInfo InspectHub(Type hubType, string route, bool sendEnumByString = false)
    {
        return _hubCache.GetOrAdd(hubType, type =>
        {
            var hubDocAttr = type.GetCustomAttribute<HubDocumentationAttribute>();
            var authorizeAttr = type.GetCustomAttribute<AuthorizeAttribute>();

            var hubInfo = new HubInfo
            {
                Name = hubDocAttr?.Name ?? type.Name.Replace("Hub", ""),
                Description = hubDocAttr?.Description ?? $"SignalR Hub: {type.Name}",
                Route = route,
                RequiresAuth = authorizeAttr != null
            };

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && m.DeclaringType == type && !IsFrameworkMethod(m));

            foreach (var method in methods)
            {
                var methodInfo = InspectMethod(method, sendEnumByString);
                hubInfo.Methods.Add(methodInfo);
            }

            return hubInfo;
        });
    }

    private HubMethodInfo InspectMethod(MethodInfo method, bool sendEnumByString)
    {
        var docAttr = method.GetCustomAttribute<HubMethodDocumentationAttribute>();
        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();

        var methodInfo = new HubMethodInfo
        {
            Name = method.Name,
            Summary = docAttr?.Summary ?? method.Name,
            Description = docAttr?.Description ?? $"Hub method: {method.Name}",
            Tags = docAttr?.Tags ?? new string[0],
            ReturnType = GetFriendlyTypeName(method.ReturnType),
            RequiresAuth = authorizeAttr != null,
            ReturnSchema = GenerateSchema(GetActualReturnType(method.ReturnType), sendEnumByString)
        };

        if (authorizeAttr != null)
        {
            methodInfo.RequiredRoles = authorizeAttr.Roles?.Split(',').Select(r => r.Trim()).ToArray() ?? new string[0];
            methodInfo.RequiredPolicies = !string.IsNullOrEmpty(authorizeAttr.Policy)
                ? new[] { authorizeAttr.Policy }
                : new string[0];
        }

        foreach (var param in method.GetParameters())
        {
            if (param.ParameterType == typeof(CancellationToken))
                continue;

            methodInfo.Parameters.Add(InspectParameter(param, sendEnumByString));
        }

        return methodInfo;
    }

    private Type GetActualReturnType(Type returnType)
    {
        if (returnType == typeof(void))
            return returnType;

        if (returnType == typeof(Task))
            return typeof(void);

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return returnType.GetGenericArguments()[0];
        }

        if (returnType == typeof(ValueTask))
            return typeof(void);

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            return returnType.GetGenericArguments()[0];
        }

        return returnType;
    }

    private HubParameterInfo InspectParameter(ParameterInfo param, bool sendEnumByString)
    {
        var paramInfo = new HubParameterInfo
        {
            Name = param.Name ?? string.Empty,
            Type = GetFriendlyTypeName(param.ParameterType),
            IsOptional = param.IsOptional,
            DefaultValue = param.DefaultValue,
            Description = $"Parameter of type {GetFriendlyTypeName(param.ParameterType)}",
            Schema = GenerateSchema(param.ParameterType, sendEnumByString),
            SendEnumAsString = sendEnumByString
        };

        return paramInfo;
    }

    private object? GenerateSchema(Type type, bool enumAsString)
    {
        return GenerateSchema(type, new HashSet<Type>(), enumAsString);
    }

    private bool ShouldSkipType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (underlyingType.Namespace?.StartsWith("System") == true)
        {
            var allowedSystemTypes = new[]
            {
                typeof(string),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(Guid),
                typeof(TimeSpan),
                typeof(IEnumerable<>),
                typeof(Task<>),
                typeof(IAsyncEnumerable<>)
            };

            if (underlyingType.IsGenericType)
            {
                var genericTypeDef = underlyingType.GetGenericTypeDefinition();
                if (allowedSystemTypes.Contains(genericTypeDef))
                {
                    return false;
                }
            }

            if (!allowedSystemTypes.Contains(underlyingType) &&
                !underlyingType.IsPrimitive &&
                !underlyingType.IsEnum)
            {
                return true;
            }
        }

        if (underlyingType.Namespace?.StartsWith("Microsoft") == true)
            return true;

        if (underlyingType.Namespace?.StartsWith("System.Reflection") == true)
            return true;

        return false;
    }

    private object? GenerateSchema(Type type, HashSet<Type> visitedTypes, bool enumAsString)
    {
        if (ShouldSkipType(type))
            return null;

        if (IsSimpleType(type) && !type.IsEnum && !(Nullable.GetUnderlyingType(type)?.IsEnum == true))
            return null;

        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        try
        {
            if (underlyingType == typeof(void))
                return null;

            if (underlyingType.IsGenericType && (underlyingType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>) || underlyingType.GetGenericTypeDefinition() == typeof(ChannelReader<>)))
            {
                var itemType = underlyingType.GetGenericArguments()[0];
                return new
                {
                    type = "stream",
                    description = "Server-to-client streaming",
                    itemSchema = GenerateSchema(itemType, visitedTypes, enumAsString) ?? new { type = GetJsonType(itemType) }
                };
            }


            if (underlyingType.IsEnum)
            {
                var enumValues = Enum.GetNames(underlyingType)
                    .Select(name => new
                    {
                        name = name,
                        value = Convert.ToInt32(Enum.Parse(underlyingType, name))
                    })
                    .ToArray();

                return new
                {
                    type = "string",
                    @enum = enumValues.Select(e => e.name).ToArray(),
                    enumValues = enumValues,
                    description = $"Enum values: {string.Join(", ", enumValues.Select(e => $"{e.name}({e.value})"))}"
                };
            }

            if (visitedTypes.Contains(underlyingType))
            {
                return new { type = GetJsonType(underlyingType) };
            }

            visitedTypes.Add(underlyingType);

            if (underlyingType.IsGenericType)
            {
                var genericDef = underlyingType.GetGenericTypeDefinition();
                if (genericDef == typeof(List<>) || genericDef == typeof(IEnumerable<>) || genericDef == typeof(Task<>))
                {
                    var itemType = underlyingType.GetGenericArguments()[0];
                    var result = new
                    {
                        type = "array",
                        items = GenerateSchema(itemType, visitedTypes, enumAsString) ?? new { type = GetJsonType(itemType) }
                    };
                    visitedTypes.Remove(underlyingType);
                    return result;
                }
            }

            if (underlyingType.IsClass && underlyingType != typeof(string))
            {
                var properties = new Dictionary<string, object>();
                var required = new List<string>();

                foreach (var prop in underlyingType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propSchema = GenerateSchema(prop.PropertyType, visitedTypes, enumAsString);
                    if (propSchema != null)
                    {
                        properties[ToCamelCase(prop.Name)] = propSchema;
                    }
                    else
                    {
                        properties[ToCamelCase(prop.Name)] = new { type = GetJsonType(prop.PropertyType) };
                    }

                    if (!IsNullable(prop.PropertyType))
                    {
                        required.Add(ToCamelCase(prop.Name));
                    }
                }

                var schema = new Dictionary<string, object>
                {
                    ["type"] = "object",
                    ["properties"] = properties
                };

                if (required.Any())
                {
                    schema["required"] = required;
                }

                visitedTypes.Remove(underlyingType);
                return schema;
            }

            visitedTypes.Remove(underlyingType);
            return new { type = GetJsonType(underlyingType) };
        }
        catch
        {
            visitedTypes.Remove(underlyingType);
            return null;
        }
    }

    private bool IsSimpleType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return underlyingType.IsPrimitive ||
           underlyingType == typeof(string) ||
           underlyingType == typeof(DateTime) ||
           underlyingType == typeof(DateTimeOffset) ||
           underlyingType == typeof(Guid) ||
           underlyingType == typeof(decimal) ||
           underlyingType == typeof(TimeSpan) ||
           underlyingType == typeof(Uri) ||
           (underlyingType.Namespace?.StartsWith("System") == true && underlyingType.IsValueType);
    }

    private bool IsNullable(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    private string GetJsonType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(string))
            return "string";
        if (underlyingType == typeof(int) || underlyingType == typeof(long) || underlyingType == typeof(short))
            return "integer";
        if (underlyingType == typeof(float) || underlyingType == typeof(double) || underlyingType == typeof(decimal))
            return "number";
        if (underlyingType == typeof(bool))
            return "boolean";
        if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
            return "string";
        if (underlyingType == typeof(Guid))
            return "string";
        if (underlyingType.IsEnum)
            return "string";

        return "object";
    }

    private string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    private string GetFriendlyTypeName(Type type)
    {
        if (type == typeof(void))
            return "void";

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(Task<>))
            {
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}";
            } else if (genericDef == typeof(IEnumerable<>) || genericDef == typeof(List<>))
            {
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}[]";
            } else if(genericDef == typeof(IAsyncEnumerable<>))
            {
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}";
            }

            var typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return $"{typeName}<{args}>";
        }

        return type.Name;
    }
}
