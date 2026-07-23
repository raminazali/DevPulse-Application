namespace DevPulse.Core.ValueObjects;

public static class ErrorFingerprint
{
    public static string Create(string message, string stackTrace, string url)
    {
        var raw = $"{message}-{stackTrace}-{url}";
        return raw.GetHashCode().ToString();
    }
}
