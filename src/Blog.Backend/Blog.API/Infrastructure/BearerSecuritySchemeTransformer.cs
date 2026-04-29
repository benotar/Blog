using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Blog.API.Infrastructure;

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
    private const string BearerAuthenticationScheme = "Bearer";

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

        if (schemes.All(s => s.Name != BearerAuthenticationScheme))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            [BearerAuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http, Scheme = "bearer", In = ParameterLocation.Header, BearerFormat = "JWT"
            }
        };
    }
}