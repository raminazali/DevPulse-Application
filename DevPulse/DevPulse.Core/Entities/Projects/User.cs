using DevPulse.Core.Entities.Base;
using DevPulse.Core.Interfaces;
using System.Net.Mail;

namespace DevPulse.Core.Entities.Projects;

public class User : IdentifierModel, IEntity
{
    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string FullName { get; private set; } = null!;

    public bool IsAdmin { get; private set; } = false;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    // Constructor خصوصی برای EF Core
    private User() { }

    /// <summary>
    /// Constructor اصلی برای ایجاد کاربر جدید
    /// </summary>
    public User(string email, string fullName, bool isAdmin = false)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName cannot be empty.", nameof(fullName));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format.", nameof(email));

        Email = email.Trim().ToLowerInvariant();
        FullName = fullName.Trim();
        IsAdmin = isAdmin;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        Projects = new List<Project>();
    }

    /// <summary>
    /// تغییر ایمیل
    /// </summary>
    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty.", nameof(newEmail));

        if (!IsValidEmail(newEmail))
            throw new ArgumentException("Invalid email format.", nameof(newEmail));

        Email = newEmail.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("PasswordHash cannot be empty.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// تغییر وضعیت فعال/غیرفعال
    /// </summary>
    public void ChangeStatus(bool isActive)
    {
        if (IsActive == isActive)
            return;

        IsActive = isActive;
    }

    /// <summary>
    /// ثبت زمان آخرین لاگین
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ارتقا به ادمین
    /// </summary>
    public void MakeAdmin()
    {
        IsAdmin = true;
    }

    /// <summary>
    /// حذف دسترسی ادمین
    /// </summary>
    public void RemoveAdmin()
    {
        IsAdmin = false;
    }

    /// <summary>
    /// اعتبارسنجی ساده ایمیل
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// به‌روزرسانی نام کامل
    /// </summary>
    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName cannot be empty.", nameof(fullName));

        FullName = fullName.Trim();
    }
    /// <summary>
    /// به‌روزرسانی چند فیلد همزمان
    /// </summary>
    public void Update(string fullName, string? email = null, bool? isActive = null)
    {
        UpdateFullName(fullName);

        if (!string.IsNullOrWhiteSpace(email))
            ChangeEmail(email);

        if (isActive.HasValue)
            ChangeStatus(isActive.Value);
    }
}
