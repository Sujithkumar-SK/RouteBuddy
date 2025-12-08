using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Common.Errors
{
    public class UserErrors
    {
        public static Error NotFound(Guid userId) =>
            Error.NotFound("Users.NotFound", $"The user with the Id = '{userId}' was not found");

        public static Error EmailAlreadyExists(string email) =>
            Error.Conflict(
                "Users.EmailAlreadyExists",
                $"The email '{email}' is already associated with another user."
            );
    }
}
