namespace Blog.Domain.Enums;

public enum ErrorCode
{
    RefreshTokenHasExpired,
    InvalidRefreshToken,
    InvalidCredentials,
    UserAlreadyExists,
    InvalidModel,
    UnexpectedError
}