namespace DevPulse.Core.Exceptions;

public abstract class DevPulseException : Exception
{
    public string Code { get; }
    public string Title { get; }

    protected DevPulseException(string message, string code, string title, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        Title = title;
    }
}
