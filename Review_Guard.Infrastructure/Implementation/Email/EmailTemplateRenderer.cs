using System.Reflection;

namespace Review_Guard.Infrastructure.Implementation.Email;

public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly string _basePath;

    public EmailTemplateRenderer()
    {
        var currentAssemblyPath =
            Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location)!;

        _basePath = Path.Combine(currentAssemblyPath, "EmailTemplates");
    }

    public async Task<string> RenderAsync(
        string templateName,
        Dictionary<string, string> data)
    {
        var templatePath =
            Path.Combine(_basePath, $"{templateName}.html");

        var layoutPath =
            Path.Combine(_basePath, "Layout.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException(
                $"Template not found: {templatePath}");

        if (!File.Exists(layoutPath))
            throw new FileNotFoundException(
                $"Layout not found: {layoutPath}");

        var template =
            await File.ReadAllTextAsync(templatePath);

        var layout =
            await File.ReadAllTextAsync(layoutPath);

        foreach (var item in data)
        {
            template = template.Replace(
                $"{{{{{item.Key}}}}}",
                item.Value);
        }

        layout = layout.Replace("{{content}}", template);
        layout = layout.Replace("{{year}}",
            DateTime.UtcNow.Year.ToString());

        return layout;
    }
}