namespace DevPulse.Core.Exceptions;

public class ForbiddenException : DevPulseException
{
    public ForbiddenException(string message, string? code = null)
        : base(message, code ?? "FORBIDDEN", "Access denied") { }
}
