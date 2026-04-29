namespace Blog.Domain.Enums;

public enum ErrorCode
{
    // Auth
    UserNotFound,
    UserAlreadyExist,
    InvalidCredentials,
    UserUnauthenticated,
    AccessDenied,

    // Validation
    InvalidRequest,
    InvalidModel,
    QueryUserIdMissing,

    // Tokens
    InvalidRefreshToken,
    RefreshTokenHasExpired,

    // Comments
    CommentNotFound,
    NothingToDelete,
    NothingToUpdate,

    // Posts
    PostNotFound,
    PostTitleAlreadyExist,

    // User Validation
    UsernameAlreadyExist,
    EmailAlreadyExist,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,

    // Unexpected Error
    UnexpectedError
}