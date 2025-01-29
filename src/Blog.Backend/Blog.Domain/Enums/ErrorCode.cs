namespace Blog.Domain.Enums;

public enum ErrorCode
{
    RefreshTokenHasExpired,
    InvalidRefreshToken,
    InvalidCredentials,
    UserAlreadyExists,
    InvalidModel,
    ThereIsNothingToDelete,
    NothingToUpdate,
    NothingToDelete,
    UserIdMissing,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,
    UnexpectedError
}