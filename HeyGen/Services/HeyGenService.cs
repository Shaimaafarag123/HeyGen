using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HeyGen.Data;
using HeyGen.Models;

namespace HeyGen.Services
{
    public class HeyGenService : IHeyGenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<HeyGenService> _logger;

        public HeyGenService(
            HttpClient httpClient,
            IConfiguration configuration,
            AppDbContext dbContext,
            ILogger<HeyGenService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        private string GetApiKey() => _configuration["HeyGen:ApiKey"];
        private string GetBaseUrl() => _configuration["HeyGen:BaseUrl"];

        public async Task<HeyGenVideoResponse> CreateVideoAsync(HeyGenVideoRequest request)
        {
            try
            {
                string apiUrl = $"{GetBaseUrl()}/v2/video/generate";

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                };

                string jsonRequest = JsonSerializer.Serialize(request, jsonOptions);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                httpRequest.Headers.Add("x-api-key", GetApiKey());
                httpRequest.Content = content;

                _logger.LogInformation("Sending request to HeyGen API: {Request}", jsonRequest);
                var response = await _httpClient.SendAsync(httpRequest);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from HeyGen API: {ResponseContent}", responseContent);

                var heyGenResponse = JsonSerializer.Deserialize<HeyGenVideoResponse>(responseContent, jsonOptions);

                await StoreVideoRequestAsync(request, heyGenResponse, jsonRequest);

                return heyGenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating video with HeyGen API");
                throw;
            }
        }

        private async Task StoreVideoRequestAsync(HeyGenVideoRequest request, HeyGenVideoResponse response, string jsonRequest)
        {
            try
            {
                var videoRequestEntity = new VideoRequestEntity
                {
                    HeyGenVideoId = response?.VideoId ?? string.Empty,
                    Title = request.Title ?? "Untitled",
                    Caption = request.Caption,
                    Width = request.Dimension?.Width ?? 0,
                    Height = request.Dimension?.Height ?? 0,
                    JsonRequest = jsonRequest,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.VideoRequests.Add(videoRequestEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing video request in database");
            }
        }

        public async Task<AvatarsResponse> GetAvatarsAsync()
        {
            try
            {
                string apiUrl = $"{GetBaseUrl()}/v2/avatars";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                httpRequest.Headers.Add("x-api-key", GetApiKey());

                _logger.LogInformation("Sending request to HeyGen API to fetch avatars");

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to fetch avatars from HeyGen API. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode(); 
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from HeyGen API: {ResponseContent}", responseContent);

                using JsonDocument doc = JsonDocument.Parse(responseContent);
                JsonElement root = doc.RootElement;

                var avatars = new List<Avatar>();
                // object
                //loop avaters ...> read avaterId ,name, geneder
                //assign AvaterResponse (Avatars) which has  (AvatarId, AvatarName,  Gender )
                foreach (var avatarJson in root.GetProperty("data").GetProperty("avatars").EnumerateArray())
                {
                    var avatar = new Avatar
                    {
                        AvatarId = avatarJson.GetProperty("avatar_id").GetString(),
                        AvatarName = avatarJson.GetProperty("avatar_name").GetString(),
                        Gender = avatarJson.GetProperty("gender").GetString()
                    };
                    avatars.Add(avatar);
                }

                return new AvatarsResponse
                {
                    Avatars = avatars
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching avatars from HeyGen API");
                throw;
            }
        }





        public async Task<VoicesResponse> GetVoicesAsync()
        {
            try
            {
                string apiUrl = $"{GetBaseUrl()}/v2/voices";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                httpRequest.Headers.Add("x-api-key", GetApiKey());

                _logger.LogInformation("Sending request to HeyGen API to fetch voices");
                var response = await _httpClient.SendAsync(httpRequest);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received voices response from HeyGen API: {ResponseContent}", responseContent);

                using JsonDocument doc = JsonDocument.Parse(responseContent);
                JsonElement root = doc.RootElement;

                var voices = new List<Voice>();
                foreach (var voiceJson in root.GetProperty("data").GetProperty("voices").EnumerateArray())
                {
                    var voice = new Voice
                    {
                        VoiceId = voiceJson.GetProperty("voice_id").GetString(),
                        Language = voiceJson.GetProperty("language").GetString(),
                        Gender = voiceJson.GetProperty("gender").GetString()
                    };
                    voices.Add(voice);
                }

                return new VoicesResponse
                {
                    Voices = voices
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching voices from HeyGen API");
                throw;
            }
        }
    }
}
