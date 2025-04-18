using Xunit;
using System.IO;
using LapStore.BLL.Services;
using Microsoft.AspNetCore.Hosting;
using FakeItEasy;

namespace LapStore.Test.ServicesTest.FileServiceTest
{
    public class DeletePhysicalFileTest
    {
        private readonly FileService _fileService;
        private readonly IWebHostEnvironment _fakeEnv;

        public DeletePhysicalFileTest()
        {
            _fakeEnv = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => _fakeEnv.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            _fileService = new FileService(_fakeEnv);
        }

        [Fact]
        public void DeletePhysicalFile_FileExistsButNotDirectory_ReturnsTrue()
        {
            // Arrange
            var testFileName = "test_delete_me.txt";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", testFileName);

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            File.WriteAllText(filePath, "This file will be deleted.");

            // Act
            var result = _fileService.DeletePhysicalFile(testFileName);

            // Assert
            Assert.True(result);
            Assert.False(File.Exists(filePath)); // Make sure file is actually deleted
        }

        [Fact]
        public void DeletePhysicalFile_PathIsDirectory_ReturnsFalse()
        {
            // Arrange
            var testFolder = "my_test_folder";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", testFolder);
            Directory.CreateDirectory(folderPath); // Simulate a directory

            // Act
            var result = _fileService.DeletePhysicalFile(testFolder);

            // Assert
            Assert.False(result);

            // Cleanup
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath);
        }
    }
}
