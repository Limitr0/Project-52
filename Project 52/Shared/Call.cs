namespace Shared.Models;

public class Call
{
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CallTime { get; set; }
    public bool IsIncoming { get; set; }
}
