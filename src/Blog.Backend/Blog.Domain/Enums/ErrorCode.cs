namespace Blog.Domain.Enums;

public enum ErrorCode
{
    // Auth
    UserNotFound,
    UserAlreadyExists,
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
    PostTitleAlreadyExists,

    // User Validation
    UsernameAlreadyExists,
    EmailAlreadyExists,
    EnterYourCurrentAndNewPassword,
    PasswordDontMatch,

    // Unexpected Error
    UnexpectedError
}