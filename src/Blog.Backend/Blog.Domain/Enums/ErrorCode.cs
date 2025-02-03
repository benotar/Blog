namespace Blog.Domain.Enums;

public enum ErrorCode
{
    UserNotFound,
    PostTitleAlreadyExists,
    RefreshTokenHasExpired,
    InvalidRefreshToken,
    InvalidCredentials,
    UserAlreadyExists,
    InvalidModel,
    ThereIsNothingToDelete,
    NothingToUpdate,
    NothingToDelete,
    UserIdMissing,
    InvalidUserId,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,
    UnexpectedError
}