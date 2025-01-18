using Azure.AI.Translation.Text;
using Blog.Application.Interfaces.Services;

namespace Blog.Application.Services;

public class AzureTranslatorService : IAzureTranslatorService
{
    private readonly TextTranslationClient _client;

    public AzureTranslatorService(TextTranslationClient client)
    {
        _client = client;
    }

    public async Task<string?> TranslateAsync(string textToTranslate)
    {
        var response = await _client.TranslateAsync("en", textToTranslate);
        var translations = response.Value;
        var translation = translations[0];
        var textResult = translation?.Translations?[0].Text;
        return textResult;
    }
}