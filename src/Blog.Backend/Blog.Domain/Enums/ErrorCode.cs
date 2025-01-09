namespace Blog.Domain.Enums;

public enum ErrorCode
{
    UnexpectedAuthenticationError,
    JwtTokenIsUndefined,
    InvalidCredentials,
    UserAlreadyExists,
    InvalidModel,
    UnexpectedError
}