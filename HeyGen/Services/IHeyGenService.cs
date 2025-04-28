using HeyGen.Models;

namespace HeyGen.Services
{
    public interface IHeyGenService
    {
        Task<HeyGenVideoResponse> CreateVideoAsync(HeyGenVideoRequest request);
        Task<AvatarsResponse> GetAvatarsAsync();
        Task<VoicesResponse> GetVoicesAsync();
    }
}
