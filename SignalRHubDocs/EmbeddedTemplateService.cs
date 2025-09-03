using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SignalRHubDocs;
public class EmbeddedTemplateService
{
    private readonly ConcurrentDictionary<string, string> _templateCache = new();
    private readonly Assembly _assembly;

    public EmbeddedTemplateService()
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    public async Task<string> LoadTemplateAsync(string templateName)
    {
        return await Task.Run(() => _templateCache.GetOrAdd(templateName, LoadEmbeddedTemplate));
    }

    private string LoadEmbeddedTemplate(string templateName)
    {
        var resourceName = $"SignalRHubDocs.Templates.{templateName}.html";

        using var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded template '{resourceName}' not found");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public string ProcessTemplate(string template, Dictionary<string, string> replacements)
    {
        var result = template;
        foreach (var replacement in replacements)
        {
            result = result.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }
        return result;
    }
}
