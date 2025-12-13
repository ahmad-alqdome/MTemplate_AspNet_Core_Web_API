namespace MyTemplate.Entities;
public class RefreshToken
{

    public int Id { get; set; }

    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? Device { get; set; }
    public string? RevokedReason { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn == null && !IsExpired;
    public Guid UserId { get; set; } 

   

}
