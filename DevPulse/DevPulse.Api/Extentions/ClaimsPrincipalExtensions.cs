using DevPulse.Core.Exceptions;
using System.Security.Claims;

namespace DevPulse.Api.Extentions;


public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// گرفتن User ID از Token
    /// </summary>
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        var userIdInt = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdInt == null)
            throw new UnauthorizedException("شناسه کاربر یافت نشد");
        return userIdInt;
    }

    /// <summary>
    /// بررسی اینکه کاربر ادمین است
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }

    /// <summary>
    /// گرفتن User ID به صورت int
    /// </summary>
    public static Guid GetUserIdAsGuid(this ClaimsPrincipal user)
    {
        var userIdString = user.GetUserId();
        Guid.TryParse(userIdString, out Guid userId);
        return userId;
    }

    /// <summary>
    /// بررسی اینکه آیا کاربر لاگین کرده است
    /// </summary>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }
}

