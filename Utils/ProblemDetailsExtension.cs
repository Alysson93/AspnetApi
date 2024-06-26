using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;

namespace AspnetApi.Utils;

public static class ProblemDetailsExtension
{
    public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications) {
        return notifications.GroupBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Message)
            .ToArray());
    }

    public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> errors) {
        var dictionary = new Dictionary<string, string[]>
        {
            { "Error", errors.Select(e => e.Description).ToArray() }
        };
        return dictionary;
    }
}
