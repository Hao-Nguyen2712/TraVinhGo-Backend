// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Admins;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitTest.Admins
{
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _mockAdminRepository;
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<ISpeedSmsService> _mockSpeedSmsService;
        private readonly Mock<IBaseRepository<Otp>> _mockOtpRepository;
        private readonly AdminService _adminService;
        private const string CacheKey = "otp_context:";

        public AdminServiceTests()
        {
            _mockAdminRepository = new Mock<IAdminRepository>();
            _mockRoleService = new Mock<IRoleService>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockCacheService = new Mock<ICacheService>();
            _mockSpeedSmsService = new Mock<ISpeedSmsService>();
            _mockOtpRepository = new Mock<IBaseRepository<Otp>>();

            _adminService = new AdminService(
                _mockAdminRepository.Object,
                _mockEmailSender.Object,
                _mockRoleService.Object,
                _mockCacheService.Object,
                _mockSpeedSmsService.Object,
                _mockOtpRepository.Object
            );
        }

        [Fact]
        public async Task UpdateSetting_WithValidEmail_ShouldUpdateUserEmailAndReturnTrue()
        {
            // Arrange
            var userId = "user123";
            var newEmail = "newemail@example.com";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "email",
                UpdateValue = newEmail
            };

            var user = new User
            {
                Id = userId,
                Email = "oldemail@example.com",
                Username = "TestUser",
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAdminRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminService.UpdateSetting(request, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(newEmail, user.Email);
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Email == newEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSetting_WithValidPhoneNumber_ShouldUpdateUserPhoneAndReturnTrue()
        {
            // Arrange
            var userId = "user123";
            var newPhone = "+84123456789";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "phoneNumber",
                UpdateValue = newPhone
            };

            var user = new User
            {
                Id = userId,
                PhoneNumber = "+84987654321",
                Username = "TestUser",
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAdminRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminService.UpdateSetting(request, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(newPhone, user.PhoneNumber);
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.PhoneNumber == newPhone), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSetting_WithInvalidEmail_ShouldReturnFalse()
        {
            // Arrange
            var userId = "user123";
            var invalidEmail = "not-an-email";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "email",
                UpdateValue = invalidEmail
            };

            var user = new User
            {
                Id = userId,
                Email = "oldemail@example.com",
                Username = "TestUser",
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _adminService.UpdateSetting(request, userId);

            // Assert
            Assert.False(result);
            Assert.Equal("oldemail@example.com", user.Email); // Email should not change
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSetting_WithInvalidPhoneNumber_ShouldThrowArgumentException()
        {
            // Arrange
            var userId = "user123";
            var invalidPhone = "not-a-phone";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "phoneNumber",
                UpdateValue = invalidPhone
            };

            var user = new User
            {
                Id = userId,
                PhoneNumber = "+84987654321",
                Username = "TestUser",
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _adminService.UpdateSetting(request, userId));

            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSetting_WithInvalidUpdateType_ShouldThrowArgumentException()
        {
            // Arrange
            var userId = "user123";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "username", // Not a valid update type
                UpdateValue = "newUsername"
            };

            var user = new User
            {
                Id = userId,
                Username = "TestUser",
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _adminService.UpdateSetting(request, userId));

            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSetting_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = "nonexistent";
            var request = new UpdateAdminSettingRequest
            {
                UpdateType = "email",
                UpdateValue = "newemail@example.com"
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _adminService.UpdateSetting(request, userId));

            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ResendOtpForUpdate_WithValidEmailAndContext_ShouldGenerateNewOtp()
        {
            // Arrange
            string identifier = "test@example.com";
            string context = "old-context-id";
            string oldOtpId = "old-otp-id";
            var cancellationToken = CancellationToken.None;

            // Setup cache to return old OTP ID
            _mockCacheService.Setup(c => c.GetData<string>(CacheKey + context))
                .ReturnsAsync(oldOtpId);

            // Setup OTP repository to return old OTP entity
            var oldOtp = new Otp
            {
                Id = oldOtpId,
                Identifier = identifier,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                ExpiredAt = DateTime.UtcNow.AddMinutes(3),
                HashedOtpCode = "old-hashed-code",
                IdentifierType = "verify update settings",
                IsUsed = false,
                AttemptCount = 0
            };

            _mockOtpRepository.Setup(r => r.GetByIdAsync(oldOtpId, cancellationToken))
                .ReturnsAsync(oldOtp);

            // Setup cache and repository for delete operations
            _mockCacheService.Setup(c => c.RemoveData(CacheKey + context))
                .Returns(Task.CompletedTask);

            _mockOtpRepository.Setup(r => r.DeleteAsync(oldOtp, cancellationToken))
                .Returns(Task.CompletedTask);

            // Setup repository for adding new OTP
            _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), cancellationToken))
                .ReturnsAsync((Otp otp, CancellationToken _) => otp);

            // Setup email sender
            _mockEmailSender.Setup(e => e.SendEmailAsync(
                identifier,
                "OTP Verification For TraVinhGo",
                It.IsAny<string>(),
                cancellationToken))
                .Returns(Task.CompletedTask);

            // Setup cache for storing new OTP context
            _mockCacheService.Setup(c => c.SetData(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null))
                .Returns(Task.CompletedTask);

            // Act
            var newContextId = await _adminService.ResendOtpForUpdate(identifier, context, cancellationToken);

            // Assert
            Assert.NotNull(newContextId);
            Assert.NotEqual(context, newContextId);

            // Verify old OTP was retrieved from cache
            _mockCacheService.Verify(c => c.GetData<string>(CacheKey + context), Times.Once);

            // Verify old OTP was retrieved from repository
            _mockOtpRepository.Verify(r => r.GetByIdAsync(oldOtpId, cancellationToken), Times.Once);

            // Verify old OTP was deleted
            _mockCacheService.Verify(c => c.RemoveData(CacheKey + context), Times.Once);
            _mockOtpRepository.Verify(r => r.DeleteAsync(oldOtp, cancellationToken), Times.Once);

            // Verify email was sent
            _mockEmailSender.Verify(e => e.SendEmailAsync(
                identifier,
                "OTP Verification For TraVinhGo",
                It.IsAny<string>(),
                cancellationToken),
                Times.Once);

            // Verify new OTP was stored
            _mockOtpRepository.Verify(r => r.AddAsync(
                It.Is<Otp>(o =>
                    o.Identifier == identifier &&
                    o.IdentifierType == "verify update settings" &&
                    !o.IsUsed),
                cancellationToken),
                Times.Once);

            // Verify new context was stored in cache
            _mockCacheService.Verify(c => c.SetData(
                It.Is<string>(s => s.StartsWith(CacheKey)),
                It.IsAny<string>(),
                null),
                Times.Once);
        }

        [Fact]
        public async Task ResendOtpForUpdate_WithPhoneNumber_ShouldSendSMS()
        {
            // Arrange
            string identifier = "+84123456789";
            string context = "old-context-id";
            string oldOtpId = "old-otp-id";
            var cancellationToken = CancellationToken.None;

            // Setup cache and repository similar to previous test
            _mockCacheService.Setup(c => c.GetData<string>(CacheKey + context))
                .ReturnsAsync(oldOtpId);

            var oldOtp = new Otp
            {
                Id = oldOtpId,
                Identifier = identifier,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                ExpiredAt = DateTime.UtcNow.AddMinutes(3),
                HashedOtpCode = "old-hashed-code",
                IdentifierType = "verify update settings",
                IsUsed = false,
                AttemptCount = 0
            };

            _mockOtpRepository.Setup(r => r.GetByIdAsync(oldOtpId, cancellationToken))
                .ReturnsAsync(oldOtp);

            _mockCacheService.Setup(c => c.RemoveData(CacheKey + context))
                .Returns(Task.CompletedTask);

            _mockOtpRepository.Setup(r => r.DeleteAsync(oldOtp, cancellationToken))
                .Returns(Task.CompletedTask);

            _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), cancellationToken))
                .ReturnsAsync((Otp otp, CancellationToken _) => otp);

            // Setup SMS service instead of email
            _mockSpeedSmsService.Setup(s => s.SendSMS(
                identifier,
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.SetData(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null))
                .Returns(Task.CompletedTask);

            // Act
            var newContextId = await _adminService.ResendOtpForUpdate(identifier, context, cancellationToken);

            // Assert
            Assert.NotNull(newContextId);

            // Verify SMS was sent
            _mockSpeedSmsService.Verify(s => s.SendSMS(
                identifier,
                It.IsAny<string>()),
                Times.Once);

            // Email should NOT be sent for phone numbers
            _mockEmailSender.Verify(e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ResendOtpForUpdate_WithInvalidContext_ShouldThrowNotFoundException()
        {
            // Arrange
            string identifier = "test@example.com";
            string invalidContext = "invalid-context";
            var cancellationToken = CancellationToken.None;

            // Setup cache to return null for invalid context
            _mockCacheService.Setup(c => c.GetData<string>(CacheKey + invalidContext))
                .ReturnsAsync((string)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _adminService.ResendOtpForUpdate(identifier, invalidContext, cancellationToken));

            Assert.Equal("OTP context not found.", exception.Message);

            // Verify no further actions were taken
            _mockOtpRepository.Verify(r => r.GetByIdAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _mockEmailSender.Verify(e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
            _mockSpeedSmsService.Verify(s => s.SendSMS(
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ResendOtpForUpdate_WithValidContextButInvalidOtpId_ShouldThrowNotFoundException()
        {
            // Arrange
            string identifier = "test@example.com";
            string context = "valid-context";
            string invalidOtpId = "invalid-otp-id";
            var cancellationToken = CancellationToken.None;

            // Setup cache to return invalid OTP ID
            _mockCacheService.Setup(c => c.GetData<string>(CacheKey + context))
                .ReturnsAsync(invalidOtpId);

            // Setup repository to return null for invalid OTP ID
            _mockOtpRepository.Setup(r => r.GetByIdAsync(invalidOtpId, cancellationToken))
                .ReturnsAsync((Otp)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _adminService.ResendOtpForUpdate(identifier, context, cancellationToken));

            Assert.Equal("OTP not found.", exception.Message);

            // Verify no further actions were taken
            _mockEmailSender.Verify(e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
            _mockSpeedSmsService.Verify(s => s.SendSMS(
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdatePassword_WithValidCredentials_ShouldUpdatePasswordAndReturnTrue()
        {
            // Arrange
            var userId = "user123";
            var currentPassword = "currentPass123";
            var newPassword = "newPass456";

            var request = new UpdateAdminPasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var currentPasswordHash = HashingExtension.HashWithSHA256(currentPassword);
            var expectedNewPasswordHash = HashingExtension.HashWithSHA256(newPassword);

            var user = new User
            {
                Id = userId,
                Email = "admin@example.com",
                Username = "TestAdmin",
                Password = currentPasswordHash,
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAdminRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminService.UpdatePassword(request, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedNewPasswordHash, user.Password);
            _mockAdminRepository.Verify(r => r.UpdateAsync(
                It.Is<User>(u => u.Password == expectedNewPasswordHash),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePassword_WithInvalidCurrentPassword_ShouldReturnFalse()
        {
            // Arrange
            var userId = "user123";
            var storedPassword = "correctPass123";
            var incorrectPassword = "wrongPass123";
            var newPassword = "newPass456";

            var request = new UpdateAdminPasswordRequest
            {
                CurrentPassword = incorrectPassword,
                NewPassword = newPassword
            };

            var storedPasswordHash = HashingExtension.HashWithSHA256(storedPassword);

            var user = new User
            {
                Id = userId,
                Email = "admin@example.com",
                Username = "TestAdmin",
                Password = storedPasswordHash,
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _adminService.UpdatePassword(request, userId);

            // Assert
            Assert.False(result);
            Assert.Equal(storedPasswordHash, user.Password); // Password should remain unchanged
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePassword_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = "nonexistent";
            var request = new UpdateAdminPasswordRequest
            {
                CurrentPassword = "currentPass",
                NewPassword = "newPass"
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _adminService.UpdatePassword(request, userId));

            Assert.Equal("User not found.", exception.Message);
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePassword_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            var userId = "user123";
            var currentPassword = "currentPass123";
            var newPassword = "newPass456";
            var cancellationToken = new CancellationToken(true);

            var request = new UpdateAdminPasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var currentPasswordHash = HashingExtension.HashWithSHA256(currentPassword);

            var user = new User
            {
                Id = userId,
                Email = "admin@example.com",
                Username = "TestAdmin",
                Password = currentPasswordHash,
                RoleId = "admin-role",
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminRepository.Setup(r => r.GetByIdAsync(userId, cancellationToken))
                .ReturnsAsync(user);

            _mockAdminRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminService.UpdatePassword(request, userId, cancellationToken);

            // Assert
            Assert.True(result);
            _mockAdminRepository.Verify(r => r.GetByIdAsync(userId, cancellationToken), Times.Once);
            _mockAdminRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), cancellationToken), Times.Once);
        }
    }
}
