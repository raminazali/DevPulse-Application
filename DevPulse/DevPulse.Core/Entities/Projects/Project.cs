using DevPulse.Core.Entities.Base;
using DevPulse.Core.Entities.Errors;
using DevPulse.Core.Interfaces;

namespace DevPulse.Core.Entities.Projects;

public class Project : IdentifierModel, IEntity
{
    public string Name { get; private set; }

    public string ApiKey { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; }

    public ICollection<ErrorLog> ErrorLogs { get; private set; } = new List<ErrorLog>();

    // Constructor خصوصی برای EF Core
    private Project() { }

    /// <summary>
    /// Constructor اصلی برای ایجاد Project جدید
    /// </summary>
    public Project(string name, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty.", nameof(name));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        Name = name.Trim();
        UserId = userId;
        ApiKey = GenerateApiKey();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        ErrorLogs = new List<ErrorLog>();
    }

    /// <summary>
    /// ویرایش نام پروژه
    /// </summary>
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Project name cannot be empty.", nameof(newName));

        Name = newName.Trim();
    }

    /// <summary>
    /// تغییر وضعیت فعال/غیرفعال
    /// </summary>
    public void ChangeStatus(bool isActive)
    {
        if (IsActive == isActive)
            return; // idempotent

        IsActive = isActive;
    }

    /// <summary>
    /// تولید ApiKey جدید
    /// </summary>
    public void RegenerateApiKey()
    {
        ApiKey = GenerateApiKey();
    }

    /// <summary>
    /// متد کمکی برای تولید ApiKey
    /// </summary>
    private static string GenerateApiKey()
    {
        return $"proj_{Guid.NewGuid():N}_{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}"
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');
    }

    /// <summary>
    /// به‌روزرسانی چند فیلد همزمان
    /// </summary>
    public void Update(string name, bool? isActive = null)
    {
        UpdateName(name);

        if (isActive.HasValue)
            ChangeStatus(isActive.Value);
    }
}
