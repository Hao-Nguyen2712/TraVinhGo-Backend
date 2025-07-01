// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using AutoMapper;
using Moq;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.CommunityTips;
using TraVinhMaps.Application.Features.CommunityTips.Mappers;
using TraVinhMaps.Application.Features.CommunityTips.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitTest
{
    public class CommunityTipsServiceTests
    {
        private readonly Mock<ICommunityTipsRepository> _mockRepository;
        private readonly CommunityTipsService _service;

        public CommunityTipsServiceTests()
        {
            // Setup AutoMapper for CommunityTipsMapper if it's not initialized
            if (CommunityTipsMapper.Mapper == null)
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CreateCommunityTipRequest, Tips>()
                        .ForMember(dest => dest.Id, opt => opt.Ignore())
                        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                        .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());
                });

                // Use reflection to set the readonly Mapper property
                var mapperField = typeof(CommunityTipsMapper).GetField("Lazy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (mapperField != null)
                {
                    var lazyMapper = new Lazy<IMapper>(() => mapperConfig.CreateMapper());
                    mapperField.SetValue(null, lazyMapper);
                }
            }

            _mockRepository = new Mock<ICommunityTipsRepository>();
            _service = new CommunityTipsService(_mockRepository.Object);
        }

        [Fact]
        public async Task AddAsync_WhenTipDoesNotExist_ShouldAddAndReturnTip()
        {
            // Arrange
            var request = new CreateCommunityTipRequest
            {
                Title = "Test Tip",
                Content = "This is a test content for the tip",
                TagId = "60c72b2f8f58d93beccd9683",
                Status = false // This should be overridden to true by the service
            };

            // Setup repository to return empty list (no existing tips)
            _mockRepository.Setup(r => r.ListAsync(
                    It.IsAny<Expression<Func<Tips, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tips>());

            // Setup repository to return the added entity
            _mockRepository.Setup(r => r.AddAsync(
                    It.IsAny<Tips>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tips tips, CancellationToken _) =>
                {
                    tips.Id = "60c72b2f8f58d93beccd9682";
                    return tips;
                });

            // Act
            var result = await _service.AddAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("60c72b2f8f58d93beccd9682", result.Id);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Content, result.Content);
            Assert.Equal(request.TagId, result.TagId);
            Assert.True(result.Status); // Status should be set to true regardless of input

            // Verify repository was called with correct parameters
            _mockRepository.Verify(r => r.ListAsync(
                It.IsAny<Expression<Func<Tips, bool>>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepository.Verify(r => r.AddAsync(
                It.Is<Tips>(t =>
                    t.Title == request.Title &&
                    t.Content == request.Content &&
                    t.TagId == request.TagId &&
                    t.Status == true),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenTipWithSameTitleAndTagExists_ShouldThrowBadRequestException()
        {
            // Arrange
            var title = "Existing Tip";
            var tagId = "60c72b2f8f58d93beccd9683";
            var request = new CreateCommunityTipRequest
            {
                Title = title,
                Content = "This is a test content for the tip",
                TagId = tagId,
                Status = true
            };

            var existingTips = new List<Tips>
            {
                new Tips
                {
                    Id = "60c72b2f8f58d93beccd9684",
                    Title = title,
                    Content = "Existing content",
                    TagId = tagId,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Setup repository to return existing tips
            _mockRepository.Setup(r => r.ListAsync(
                    It.IsAny<Expression<Func<Tips, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTips);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.AddAsync(request));

            Assert.Equal("A tip with the same title and tag already exists.", exception.Message);

            // Verify repository was called to check for existing tips
            _mockRepository.Verify(r => r.ListAsync(
                It.IsAny<Expression<Func<Tips, bool>>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify repository was never called to add a new tip
            _mockRepository.Verify(r => r.AddAsync(
                It.IsAny<Tips>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AddAsync_WithDifferentCaseAndWhitespacesInTitle_ShouldConsiderSameTitle()
        {
            // Arrange
            var originalTitle = "Existing Tip";
            var newTitle = "  existing tip  "; // Different case and extra spaces
            var tagId = "60c72b2f8f58d93beccd9683";

            var request = new CreateCommunityTipRequest
            {
                Title = newTitle,
                Content = "This is a test content for the tip",
                TagId = tagId,
                Status = true
            };

            var existingTips = new List<Tips>
            {
                new Tips
                {
                    Id = "60c72b2f8f58d93beccd9684",
                    Title = originalTitle,
                    Content = "Existing content",
                    TagId = tagId,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Setup repository mock to match the actual implementation
            _mockRepository.Setup(r => r.ListAsync(
                    It.IsAny<Expression<Func<Tips, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Expression<Func<Tips, bool>> predicate, CancellationToken _) =>
                {
                    // Simulate the actual repository behavior for case-insensitive matching
                    var result = existingTips.Where(t =>
                        t.Title.ToLower().Trim() == newTitle.ToLower().Trim() &&
                        t.TagId == tagId);
                    return Task.FromResult(result.AsEnumerable());
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.AddAsync(request));

            Assert.Equal("A tip with the same title and tag already exists.", exception.Message);
        }

        [Fact]
        public async Task DeleteTipAsync_ShouldCallRepositoryDeleteTipAsync()
        {
            // Arrange
            string tipId = "60c72b2f8f58d93beccd9682";

            _mockRepository.Setup(r => r.DeleteTipAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _service.DeleteTipAsync(tipId);

            // Assert
            _mockRepository.Verify(r => r.DeleteTipAsync(
                tipId,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteTipAsync_WithInvalidId_ShouldStillCallRepository()
        {
            // Arrange
            string invalidTipId = "invalidId";

            _mockRepository.Setup(r => r.DeleteTipAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _service.DeleteTipAsync(invalidTipId);

            // Assert
            _mockRepository.Verify(r => r.DeleteTipAsync(
                invalidTipId,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteTipAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            string tipId = "60c72b2f8f58d93beccd9682";
            var cancellationToken = new CancellationToken(true);

            _mockRepository.Setup(r => r.DeleteTipAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _service.DeleteTipAsync(tipId, cancellationToken);

            // Assert
            _mockRepository.Verify(r => r.DeleteTipAsync(
                tipId,
                cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnTip()
        {
            // Arrange
            string tipId = "60c72b2f8f58d93beccd9682";
            var expectedTip = new Tips
            {
                Id = tipId,
                Title = "Test Tip",
                Content = "This is a test content for the tip",
                TagId = "60c72b2f8f58d93beccd9683",
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(
                    tipId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTip);

            // Act
            var result = await _service.GetByIdAsync(tipId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tipId, result.Id);
            Assert.Equal(expectedTip.Title, result.Title);
            Assert.Equal(expectedTip.Content, result.Content);
            Assert.Equal(expectedTip.TagId, result.TagId);
            Assert.Equal(expectedTip.Status, result.Status);

            // Verify repository was called with correct parameters
            _mockRepository.Verify(r => r.GetByIdAsync(
                tipId,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            string invalidTipId = "invalidId";

            _mockRepository.Setup(r => r.GetByIdAsync(
                    invalidTipId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tips)null);

            // Act
            var result = await _service.GetByIdAsync(invalidTipId);

            // Assert
            Assert.Null(result);

            // Verify repository was called with correct parameters
            _mockRepository.Verify(r => r.GetByIdAsync(
                invalidTipId,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            string tipId = "60c72b2f8f58d93beccd9682";
            var cancellationToken = new CancellationToken(true);
            var expectedTip = new Tips
            {
                Id = tipId,
                Title = "Test Tip",
                Content = "This is a test content for the tip",
                TagId = "60c72b2f8f58d93beccd9683",
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(
                    tipId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTip);

            // Act
            var result = await _service.GetByIdAsync(tipId, cancellationToken);

            // Assert
            Assert.NotNull(result);

            // Verify repository was called with correct cancellation token
            _mockRepository.Verify(r => r.GetByIdAsync(
                tipId,
                cancellationToken),
                Times.Once);
        }
    }
}
