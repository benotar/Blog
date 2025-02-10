namespace Blog.Domain.Enums;

public enum ErrorCode
{
    UserNotFound,
    PostNotFound,
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
    UsernameAlreadyExists,
    EmailAlreadyExists,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,
    UnexpectedError
}