namespace DevPulse.Core.Exceptions;

public class ValidationException : DevPulseException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message, IDictionary<string, string[]>? errors = null, string? code = null)
        : base(message, code ?? "VALIDATION_ERROR", "Validation failed")
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
