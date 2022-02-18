using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Providers.Image;

public class ImageAccessor : IImageAccessor
{
    private readonly IWebHostEnvironment _environment;
    private readonly IOriginAccessor _originAccessor;
    private readonly ILogger<ImageAccessor> _logger;
    public ImageAccessor(IWebHostEnvironment environment, IOriginAccessor originAccessor, ILogger<ImageAccessor> logger)
    {
            this._logger = logger;
            this._originAccessor = originAccessor;
            this._environment = environment;
    }

    public async Task<string> AddImageAsync(IFormFile File)
    {
        var directoryPath = Path.Combine(_environment.WebRootPath, "images");

        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var fileName = $"{Guid.NewGuid()}_{File.FileName}";

        var path = Path.Combine(_environment.WebRootPath, "images", fileName);

        using var stream = new FileStream(path, FileMode.Create);

        try 
        {
            await File.CopyToAsync(stream);
            await stream.FlushAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add image");

            return null;
        }

        return fileName;
    }

    public bool DeleteImage(string fileName)
    {
        try
        {
            var directoryPath = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);

                return false;
            };

            var path = Path.Combine(_environment.WebRootPath, "images", fileName);

            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image");

            return false;
        }

        return true;
    }
}
