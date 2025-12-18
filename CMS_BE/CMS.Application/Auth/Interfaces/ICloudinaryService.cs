using Microsoft.AspNetCore.Http;

namespace CMS.Application.Auth.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(string Url, string PublicId)> UploadProfileImg(IFormFile image, string userId);
        Task<bool> DeleteFileAsync(string publicId, string userId);
    }
}