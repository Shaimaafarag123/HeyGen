using HeyGen.Models;
using HeyGen.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeyGen.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class VideoController : ControllerBase
    {
        private readonly IHeyGenService _heyGenService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(IHeyGenService heyGenService, ILogger<VideoController> logger)
        {
            _heyGenService = heyGenService;
            _logger = logger;
        }

        [HttpPost("video.create")]
        public async Task<IActionResult> CreateVideo([FromBody] HeyGenVideoRequest request)
        {
            try
            {
                _logger.LogInformation("Received video creation request");

                // Validate minimum required fields
                if (request.VideoInputs == null || !request.VideoInputs.Any())
                {
                    return BadRequest("At least one video input is required");
                }

                foreach (var input in request.VideoInputs)
                {
                    // Validate character settings
                    if (input.Character == null)
                    {
                        return BadRequest("Character settings are required");
                    }

                    if (string.IsNullOrEmpty(input.Character.AvatarId))
                    {
                        return BadRequest("Avatar ID is required");
                    }

                    // Validate voice settings
                    if (input.Voice == null)
                    {
                        return BadRequest("Voice settings are required");
                    }

                    if (string.IsNullOrEmpty(input.Voice.VoiceId))
                    {
                        return BadRequest("Voice ID is required");
                    }
                }

                // Set default values if not provided
                if (request.Dimension == null)
                {
                    request.Dimension = new Dimension { Width = 1280, Height = 720 };
                }

                var response = await _heyGenService.CreateVideoAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing video creation request");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        [HttpGet("avatars")]
        public async Task<IActionResult> GetAvatars()
        {
            try
            {
                var response = await _heyGenService.GetAvatarsAsync();
                return Ok(response);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP Error: {StatusCode} - {Message}",
                    httpEx.StatusCode, httpEx.Message);
                return StatusCode(500, $"API Error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching avatars");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("voices")]
        public async Task<IActionResult> GetVoices()
        {
            try
            {
                _logger.LogInformation("Received request to list all voices");
                var response = await _heyGenService.GetVoicesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing voices list request");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}