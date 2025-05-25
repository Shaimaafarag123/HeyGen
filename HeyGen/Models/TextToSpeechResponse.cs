namespace HeyGen.Models
{
    public class TextToSpeechResponse
    {
        public string TaskId { get; set; }
        public string Status { get; set; }
        public string AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
