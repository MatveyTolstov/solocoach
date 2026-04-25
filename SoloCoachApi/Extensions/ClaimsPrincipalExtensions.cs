using System.Security.Claims;

namespace SoloCoachApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        userId = 0;
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrEmpty(raw) && int.TryParse(raw, out userId) && userId > 0;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        if (user.TryGetUserId(out var userId))
            return userId;
        throw new InvalidOperationException("User ID not found in claims");
    }
}
