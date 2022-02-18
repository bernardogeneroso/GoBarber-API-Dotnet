using Microsoft.AspNetCore.Http;

namespace Services.Interfaces;

public interface IImageAccessor
{
    Task<string> AddImageAsync(IFormFile File);
    bool DeleteImage(string fileName);
}