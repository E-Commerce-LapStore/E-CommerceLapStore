using FakeItEasy;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LapStore.BLL.Services;

namespace LapStore.Test.ServicesTest.FileServiceTest
{
    public class UploadTest
    {
        private readonly FileService _fileService;
        private readonly IWebHostEnvironment _fakeWebHostEnv;

        public UploadTest()
        {
            _fakeWebHostEnv = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => _fakeWebHostEnv.WebRootPath).Returns(Directory.GetCurrentDirectory() + "/wwwroot");
            _fileService = new FileService(_fakeWebHostEnv);
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnsPathWithExtension()
        {
            // Arrange
            var content = "Fake file content for test";
            var fileName = "myimage.png";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var formFile = A.Fake<IFormFile>();
            A.CallTo(() => formFile.FileName).Returns(fileName);
            A.CallTo(() => formFile.Length).Returns(stream.Length);
            A.CallTo(() => formFile.OpenReadStream()).Returns(stream);
            A.CallTo(() => formFile.CopyToAsync(A<Stream>.Ignored, A<CancellationToken>.Ignored))
                .Invokes((Stream target, CancellationToken _) =>
                {
                    stream.Position = 0;
                    stream.CopyTo(target);
                })
                .Returns(Task.CompletedTask);

            var uploadPath = "/Imgs/";

            // Act
            var result = await _fileService.Upload(formFile, uploadPath);

            // Assert
            Assert.Contains(uploadPath, result);
            Assert.EndsWith(".png", result);

            // Cleanup
            var fullPath = _fakeWebHostEnv.WebRootPath + result;
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
