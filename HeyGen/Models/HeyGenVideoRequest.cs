using System.Text.Json.Serialization;

namespace HeyGen.Models
{
    public class HeyGenVideoRequest
    {
        public bool Caption { get; set; } = false;
        public string? Title { get; set; }
        public Dimension Dimension { get; set; } = new Dimension { Width = 1280, Height = 720 };
        public List<VideoInput> VideoInputs { get; set; } = new List<VideoInput>();





    }
}
