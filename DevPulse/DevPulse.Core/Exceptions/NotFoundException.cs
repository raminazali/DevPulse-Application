namespace DevPulse.Core.Exceptions;

public class NotFoundException : DevPulseException
{
    public NotFoundException(string message, string? code = null)
        : base(message, code ?? "NOT_FOUND", "Resource not found") { }
}
