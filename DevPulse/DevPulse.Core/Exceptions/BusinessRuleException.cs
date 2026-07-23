namespace DevPulse.Core.Exceptions;

public class BusinessRuleException : DevPulseException
{
    public BusinessRuleException(string message, string? code = null)
        : base(message, code ?? "BUSINESS_RULE_VIOLATION", "Business rule violation") { }
}
