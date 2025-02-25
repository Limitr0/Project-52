namespace Shared.Models;

public class Message
{
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime ReceivedTime { get; set; }
}
