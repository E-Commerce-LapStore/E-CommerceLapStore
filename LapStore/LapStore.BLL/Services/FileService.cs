using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
namespace LapStore.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> Upload(IFormFile file, string location)
        {
                try
                {
                    var path = _webHostEnvironment.WebRootPath + location;// wwwroot/Imgs
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + extension;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Save
                    using (FileStream fileStream = File.Create(path + fileName))
                    {
                        await file.CopyToAsync(fileStream);
                        fileStream.Flush();
                        return $"{location}/{fileName}";
                    }
                }
                catch
                {
                    return "Problem";
                }
        }
        public bool DeletePhysicalFile(string path)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (!Directory.Exists(directoryPath))
            {
                File.Delete(directoryPath);
                return true;
            }
            else
                return false;
        }
    }
}
