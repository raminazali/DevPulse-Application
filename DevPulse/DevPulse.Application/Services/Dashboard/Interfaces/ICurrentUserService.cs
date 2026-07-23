namespace DevPulse.Application.Services.Dashboard.Interfaces;


/// <summary>
/// Identity of the currently authenticated caller, resolved from the
/// request (e.g. JWT claims) by the hosting layer.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
}
