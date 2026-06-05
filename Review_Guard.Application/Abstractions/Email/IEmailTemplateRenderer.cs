namespace Review_Guard.Application.Abstractions.Email;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(string templateName, Dictionary<string, string> data);
}