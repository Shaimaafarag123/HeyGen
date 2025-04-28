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

        public async Task<HeyGenVideoResponse> CreateVideoAsync(HeyGenVideoRequest request)
        {
            try
            {
                string apiKey = _configuration["HeyGen:ApiKey"];
                string baseUrl = _configuration["HeyGen:BaseUrl"];
                string apiUrl = $"{baseUrl}/v2/video/generate";
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                };
                string jsonRequest = JsonSerializer.Serialize(request, jsonOptions);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Set up the HTTP request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                httpRequest.Headers.Add("x-api-key", apiKey);
                httpRequest.Content = content;
                _logger.LogInformation("Sending request to HeyGen API: {Request}", jsonRequest);
                var response = await _httpClient.SendAsync(httpRequest);

                // Ensure successful response
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from HeyGen API: {ResponseContent}", responseContent);
                var heyGenResponse = JsonSerializer.Deserialize<HeyGenVideoResponse>(responseContent, jsonOptions);

                //store request and response in database 
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
                    HeyGenVideoId = response.VideoId,
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
                string apiKey = _configuration["HeyGen:ApiKey"];
                string baseUrl = _configuration["HeyGen:BaseUrl"];
                string apiUrl = $"{baseUrl}/v2/avatars";

                //set up the HTTP request
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                httpRequest.Headers.Add("x-api-key", apiKey);

                _logger.LogInformation("Sending request to HeyGen API: {Request}");
                var response = await _httpClient.SendAsync(httpRequest);

                // Ensure successful response
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from HeyGen API: {ResponseContent}");

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var avatarsResponse = JsonSerializer.Deserialize<AvatarsResponse>(responseContent, jsonOptions);

                return avatarsResponse;
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
                string apiKey = _configuration["HeyGen:ApiKey"];
                string baseUrl = _configuration["HeyGen:BaseUrl"];
                string apiUrl = $"{baseUrl}/v2/voices";

                // Set up the HTTP request
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                httpRequest.Headers.Add("x-api-key", apiKey);
                _logger.LogInformation("Sending request to HeyGen API to fetch voices");
                var response = await _httpClient.SendAsync(httpRequest);

                // Ensure successful response
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received voices response from HeyGen API");

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var voicesResponse = JsonSerializer.Deserialize<VoicesResponse>(responseContent, jsonOptions);
                return voicesResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching voices from HeyGen API");
                throw;
            }
        }
    }
}