namespace Blog.Domain.Enums;

public enum ErrorCode
{
    JwtTokenIsUndefined,
    InvalidCredentials,
    UserAlreadyExists,
    InvalidModel,
    UnexpectedError
}