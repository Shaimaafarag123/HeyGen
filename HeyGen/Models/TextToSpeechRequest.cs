namespace HeyGen.Models
{
    public class TextToSpeechRequest
    {
        public string Text { get; set; }
        public string VoiceId { get; set; }
        public float Speed { get; set; } = 1.0f;
        public float Stability { get; set; } = 0.75f;
        public float Clarity { get; set; } = 0.75f;
        public string OutputFormat { get; set; } = "mp3";
    }
}
