namespace HeyGen.Models
{
    public class VideoRequestEntity
    {
        public int Id { get; set; }
        public string HeyGenVideoId { get; set; }
        public string Title { get; set; }
        public bool Caption { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string JsonRequest { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
