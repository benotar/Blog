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
    UserIdMissing,
    YouAreNotAllowedToAccessThisResource,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,
    UnexpectedError
}