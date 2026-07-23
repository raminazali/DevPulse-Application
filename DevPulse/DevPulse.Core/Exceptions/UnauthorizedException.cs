namespace DevPulse.Core.Exceptions;

public class UnauthorizedException : DevPulseException
{
    public UnauthorizedException(string message, string? code = null)
        : base(message, code ?? "UNAUTHORIZED", "Authentication failed") { }
}
