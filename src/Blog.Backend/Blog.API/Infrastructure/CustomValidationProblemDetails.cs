using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Infrastructure;

public class CustomValidationProblemDetails : ProblemDetails
{
    public Dictionary<string, string[]> Errors { get; set; }
}