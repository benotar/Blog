using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Blog.API.Infrastructure;

internal sealed class AuthorizeOperationTransformer : IOpenApiOperationTransformer
{
    private const string BearerAuthenticationScheme = "Bearer";


    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        var hasAllowAnonymous = metadata.OfType<IAllowAnonymous>().Any();
        var hasAuthorize = metadata.OfType<IAuthorizeData>().Any();


        if (!hasAllowAnonymous && hasAuthorize)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerAuthenticationScheme)] = []
            });
        }

        return Task.CompletedTask;
    }
}
