namespace HeyGen.Models
{
    public class HeyGenVideoResponse
    {
        public string VideoId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
