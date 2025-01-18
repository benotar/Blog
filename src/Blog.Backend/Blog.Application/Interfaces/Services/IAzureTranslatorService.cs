namespace Blog.Application.Interfaces.Services;

public interface IAzureTranslatorService
{
    Task<string?> TranslateAsync(string textToTranslate);
}