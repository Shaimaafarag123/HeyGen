namespace HeyGen.Models
{
    public class TextToSpeechEntity
    {
        public int Id { get; set; }
        public string TaskId { get; set; }
        public string Text { get; set; }
        public string VoiceId { get; set; }
        public float Speed { get; set; }
        public string OutputFormat { get; set; }
        public string Status { get; set; }
        public string AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
