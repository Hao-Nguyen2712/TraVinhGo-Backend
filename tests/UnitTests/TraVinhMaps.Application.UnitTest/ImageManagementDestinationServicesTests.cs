// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Destination;

namespace TraVinhMaps.Application.UnitTest
{
    public class ImageManagementDestinationServicesTests
    {
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly ImageManagementDestinationServices _service;

        public ImageManagementDestinationServicesTests()
        {
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _service = new ImageManagementDestinationServices(_mockCloudinaryService.Object);
        }

        [Fact]
        public async Task AddImageDestination_WithValidFiles_ShouldReturnListOfUrls()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFile("test1.jpg", 1024),
                CreateMockFile("test2.jpg", 2048)
            };

            _mockCloudinaryService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((IFormFile file) => new ImageUploadResult
                {
                    SecureUrl = new Uri($"https://cloudinary.com/{file.FileName}")
                });

            // Act
            var result = await _service.AddImageDestination(files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("https://cloudinary.com/test1.jpg", result[0]);
            Assert.Equal("https://cloudinary.com/test2.jpg", result[1]);
            _mockCloudinaryService.Verify(c => c.UploadImageAsync(It.IsAny<IFormFile>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AddImageDestination_WithEmptyFileList_ShouldReturnEmptyList()
        {
            // Arrange
            var files = new List<IFormFile>();

            // Act
            var result = await _service.AddImageDestination(files);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCloudinaryService.Verify(c => c.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task AddImageDestination_WithZeroLengthFile_ShouldReturnNull()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFile("test1.jpg", 1024),
                CreateMockFile("empty.jpg", 0)  // This should trigger the null return
            };

            _mockCloudinaryService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((IFormFile file) => new ImageUploadResult
                {
                    SecureUrl = new Uri($"https://cloudinary.com/{file.FileName}")
                });

            // Act
            var result = await _service.AddImageDestination(files);

            // Assert
            Assert.Null(result);
            // Should only try to upload the first file before finding the empty one
            _mockCloudinaryService.Verify(c => c.UploadImageAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task AddImageDestination_WhenCloudinaryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFile("test.jpg", 1024)
            };

            _mockCloudinaryService.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Upload failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.AddImageDestination(files));
        }

        [Fact]
        public async Task DeleteImageDestination_WithNullUrl_ShouldReturnFalse()
        {
            // Arrange
            string imageUrl = null;

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.False(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageDestination_WithEmptyUrl_ShouldReturnFalse()
        {
            // Arrange
            var imageUrl = string.Empty;

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.False(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageDestination_WithValidUrl_AndSuccessfulDeletion_ShouldReturnTrue()
        {
            // Arrange
            var imageUrl = "https://cloudinary.com/images/example-image.jpg";
            var expectedPublicId = "example-image";

            _mockCloudinaryService.Setup(c => c.DeleteImageAsync(expectedPublicId))
                .ReturnsAsync(new DeletionResult { Result = "ok" });

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.True(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(expectedPublicId), Times.Once);
        }

        [Fact]
        public async Task DeleteImageDestination_WithValidUrl_AndFailedDeletion_ShouldReturnFalse()
        {
            // Arrange
            var imageUrl = "https://cloudinary.com/images/example-image.jpg";
            var expectedPublicId = "example-image";

            _mockCloudinaryService.Setup(c => c.DeleteImageAsync(expectedPublicId))
                .ReturnsAsync(new DeletionResult { Result = "not found" });

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.False(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(expectedPublicId), Times.Once);
        }

        [Fact]
        public async Task DeleteImageDestination_WithInvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            var imageUrl = "not-a-valid-url";

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.False(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageDestination_WhenCloudinaryThrowsException_ShouldReturnFalse()
        {
            // Arrange
            var imageUrl = "https://cloudinary.com/images/example-image.jpg";
            var expectedPublicId = "example-image";

            _mockCloudinaryService.Setup(c => c.DeleteImageAsync(expectedPublicId))
                .ThrowsAsync(new Exception("Deletion failed"));

            // Act
            var result = await _service.DeleteImageDestination(imageUrl);

            // Assert
            Assert.False(result);
            _mockCloudinaryService.Verify(c => c.DeleteImageAsync(expectedPublicId), Times.Once);
        }

        private IFormFile CreateMockFile(string fileName, long length)
        {
            var content = new string('x', (int)length);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentDisposition).Returns($"form-data; name=\"file\"; filename=\"{fileName}\"");

            return fileMock.Object;
        }
    }
}
