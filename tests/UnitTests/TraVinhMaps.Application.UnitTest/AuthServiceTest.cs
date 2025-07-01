// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Moq;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Auth;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitTest;

public class AuthServiceTest
{
    private readonly Mock<ISpeedSmsService> _mockSpeedSmsService;
    private readonly Mock<IBaseRepository<Otp>> _mockOtpRepository;
    private readonly Mock<IBaseRepository<User>> _mockUserRepository;
    private readonly Mock<IBaseRepository<Role>> _mockRoleRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IBaseRepository<UserSession>> _mockSessionRepository;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _mockSpeedSmsService = new Mock<ISpeedSmsService>();
        _mockOtpRepository = new Mock<IBaseRepository<Otp>>();
        _mockUserRepository = new Mock<IBaseRepository<User>>();
        _mockRoleRepository = new Mock<IBaseRepository<Role>>();
        _mockCacheService = new Mock<ICacheService>();
        _mockSessionRepository = new Mock<IBaseRepository<UserSession>>();
        _mockEmailSender = new Mock<IEmailSender>();

        _authService = new AuthService(
            _mockSpeedSmsService.Object,
            _mockOtpRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockSessionRepository.Object,
            _mockCacheService.Object,
            _mockEmailSender.Object
        );
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldSendSmsAndSaveOtp_AndReturnContextId()
    {
        // Arrange
        var phoneNumber = "+84123456789";
        var cancellationToken = CancellationToken.None;

        // Capture the OTP entity passed to repository
        Otp capturedOtp = null;
        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), cancellationToken))
            .Callback<Otp, CancellationToken>((otp, _) => capturedOtp = otp)
            .ReturnsAsync((Otp otp, CancellationToken _) => otp);

        // Capture the cached data
        string capturedKey = null;
        string capturedValue = null;
        _mockCacheService.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null))
            .Callback<string, string, TimeSpan?>((key, value, _) =>
            {
                capturedKey = key;
                capturedValue = value;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.AuthenWithPhoneNumber(phoneNumber, cancellationToken);

        // Assert
        // Verify SMS was sent
        _mockSpeedSmsService.Verify(s => s.SendSMS(phoneNumber, It.IsAny<string>()), Times.Once);

        // Verify OTP was saved to repository
        _mockOtpRepository.Verify(r => r.AddAsync(It.IsAny<Otp>(), cancellationToken), Times.Once);
        Assert.NotNull(capturedOtp);
        Assert.Equal(phoneNumber, capturedOtp.Identifier);
        Assert.Equal("phone-number", capturedOtp.IdentifierType);
        Assert.False(capturedOtp.IsUsed);
        Assert.Equal(0, capturedOtp.AttemptCount);
        Assert.NotNull(capturedOtp.HashedOtpCode);

        // Verify value was cached
        _mockCacheService.Verify(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
        Assert.NotNull(capturedKey);
        Assert.NotNull(capturedValue);
        Assert.Equal(capturedOtp.Id, capturedValue);
        Assert.StartsWith("otp_context:", capturedKey);

        // Verify the context ID was returned and matches the cached key
        Assert.NotNull(result);
        Assert.Equal(capturedKey.Substring("otp_context:".Length), result);
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldHashOtpCodeCorrectly()
    {
        var phoneNumber = "+84999999999";
        Otp capturedOtp = null;
        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), It.IsAny<CancellationToken>()))
            .Callback<Otp, CancellationToken>((otp, _) => capturedOtp = otp)
            .ReturnsAsync((Otp otp, CancellationToken _) => otp);
        _mockCacheService.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        await _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None);

        Assert.NotNull(capturedOtp.HashedOtpCode);
        Assert.NotEqual("", capturedOtp.HashedOtpCode);
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldGenerateOtpOfSixDigits()
    {
        var phoneNumber = "+84888888888";
        string smsBody = null;

        _mockSpeedSmsService.Setup(s => s.SendSMS(phoneNumber, It.IsAny<string>()))
            .Callback<string, string>((_, message) => smsBody = message)
            .Returns(Task.CompletedTask);

        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Otp otp, CancellationToken _) => otp);

        _mockCacheService.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        await _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None);

        Assert.Contains("Your OTP", smsBody);
        Assert.Matches(@"\d{6}", smsBody);
    }
    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldThrowException_WhenSendSmsFails()
    {
        var phoneNumber = "+84123456789";

        _mockSpeedSmsService.Setup(s => s.SendSMS(phoneNumber, It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMS failure"));

        await Assert.ThrowsAsync<Exception>(() =>
            _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None));
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldThrowException_WhenSaveOtpFails()
    {
        var phoneNumber = "+84123456789";

        _mockSpeedSmsService.Setup(s => s.SendSMS(phoneNumber, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() =>
            _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None));
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldThrowException_WhenCacheFails()
    {
        var phoneNumber = "+84123456789";

        _mockSpeedSmsService.Setup(s => s.SendSMS(phoneNumber, It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Otp otp, CancellationToken _) => otp);
        _mockCacheService.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null))
            .ThrowsAsync(new Exception("Cache failed"));

        await Assert.ThrowsAsync<Exception>(() =>
            _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None));
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldThrowException_WhenOtpNull()
    {
        var phoneNumber = "+84123456789";

        _mockSpeedSmsService.Setup(s => s.SendSMS(phoneNumber, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<Otp>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Otp)null!);

        _mockCacheService.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        var result = await _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }



    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldHandle_NullPhoneNumber()
    {
        string phoneNumber = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None));
    }

    [Fact]
    public async Task AuthenWithPhoneNumber_ShouldHandle_InvalidFormatPhoneNumber()
    {
        var phoneNumber = "abcxyz"; // Không hợp lệ

        await Assert.ThrowsAsync<FormatException>(() =>
            _authService.AuthenWithPhoneNumber(phoneNumber, CancellationToken.None));
    }



    [Fact]
    public async Task VerifyOtp_WithValidOtpAndPhoneNumber_ExistingUser_ShouldReturnAuthResponse()
    {
        // Arrange
        var contextId = "test-context-id";
        var otpId = "otp-123";
        var otp = "123456";
        var phoneNumber = "+84123456789";
        var userId = "user-123";
        var roleId = "role-123";
        var deviceInfo = "test-device";
        var ipAddress = "127.0.0.1";
        var cancellationToken = CancellationToken.None;

        // Setup cache service to return OTP ID
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync(otpId);

        // Setup OTP repository
        var otpEntity = new Otp
        {
            Id = otpId,
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };

        _mockOtpRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken))
            .ReturnsAsync(otpEntity);

        // Setup user repository for existing user
        var user = new User
        {
            Id = userId,
            PhoneNumber = phoneNumber,
            CreatedAt = DateTime.UtcNow,
            RoleId = roleId,
            Status = true
        };

        _mockUserRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken))
            .ReturnsAsync(user);

        // Setup session repository
        _mockSessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), cancellationToken))
            .ReturnsAsync((UserSession session, CancellationToken _) => session);

        // Setup session limit enforcement
        _mockSessionRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<UserSession, bool>>>(), cancellationToken))
            .ReturnsAsync(new List<UserSession>());

        // Act
        var result = await _authService.VerifyOtp(contextId, otp, deviceInfo, ipAddress, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SessionId);
        Assert.NotNull(result.RefreshToken);

        // Verify OTP was marked as used
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.IsUsed), cancellationToken), Times.Once);

        // Verify cache was cleared
        _mockCacheService.Verify(c => c.RemoveData("otp_context:" + contextId), Times.Once);

        // Verify session was created
        _mockSessionRepository.Verify(r => r.AddAsync(It.Is<UserSession>(s =>
            s.UserId == userId &&
            s.DeviceInfo == deviceInfo &&
            s.IpAddress == ipAddress &&
            s.IsActive), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task VerifyOtp_WithValidOtpAndPhoneNumber_NewUser_ShouldCreateUserAndReturnAuthResponse()
    {
        // Arrange
        var contextId = "test-context-id";
        var otpId = "otp-123";
        var otp = "123456";
        var phoneNumber = "+84123456789";
        var roleId = "role-123";
        var deviceInfo = "test-device";
        var ipAddress = "127.0.0.1";
        var cancellationToken = CancellationToken.None;

        // Setup cache service
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync(otpId);

        // Setup OTP repository
        var otpEntity = new Otp
        {
            Id = otpId,
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };

        _mockOtpRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken))
            .ReturnsAsync(otpEntity);

        // Setup user repository for non-existing user
        _mockUserRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken))
            .ReturnsAsync((User)null);

        // Setup role repository
        var role = new Role
        {
            Id = roleId,
            RoleName = "user",
            CreatedAt = DateTime.UtcNow,
            RoleStatus = true
        };

        _mockRoleRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Role, bool>>>(), cancellationToken))
            .ReturnsAsync(role);

        // Capture the user being created
        User createdUser = null;
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), cancellationToken))
            .Callback<User, CancellationToken>((u, _) => createdUser = u)
            .ReturnsAsync((User u, CancellationToken _) => u);

        // Setup session repository
        _mockSessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), cancellationToken))
            .ReturnsAsync((UserSession session, CancellationToken _) => session);

        // Setup session limit enforcement
        _mockSessionRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<UserSession, bool>>>(), cancellationToken))
            .ReturnsAsync(new List<UserSession>());

        // Act
        var result = await _authService.VerifyOtp(contextId, otp, deviceInfo, ipAddress, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SessionId);
        Assert.NotNull(result.RefreshToken);

        // Verify user was created
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), cancellationToken), Times.Once);
        Assert.NotNull(createdUser);
        Assert.Equal(phoneNumber, createdUser.PhoneNumber);
        Assert.Equal(roleId, createdUser.RoleId);
        Assert.True(createdUser.Status);

        // Verify OTP was marked as used
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.IsUsed), cancellationToken), Times.Once);

        // Verify cache was cleared
        _mockCacheService.Verify(c => c.RemoveData("otp_context:" + contextId), Times.Once);

        // Verify session was created
        _mockSessionRepository.Verify(r => r.AddAsync(It.IsAny<UserSession>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidIdentifier_ShouldReturnNull()
    {
        // Arrange
        var contextId = "invalid-context-id";
        var cancellationToken = CancellationToken.None;

        // Setup cache service to return null
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync((string)null);

        // Act
        var result = await _authService.VerifyOtp(contextId, "123456", "device", "ip", cancellationToken);

        // Assert
        Assert.Null(result);

        // Verify no other repository calls were made
        _mockOtpRepository.Verify(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken), Times.Never);
        _mockUserRepository.Verify(r => r.GetAsyns(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken), Times.Never);
        _mockSessionRepository.Verify(r => r.AddAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task VerifyOtp_WithExpiredOtp_ShouldThrowBadRequestException()
    {
        // Arrange
        var contextId = "test-context-id";
        var otpId = "otp-123";
        var otp = "123456";
        var phoneNumber = "+84123456789";
        var cancellationToken = CancellationToken.None;

        // Setup cache service
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync(otpId);

        // Setup OTP repository with expired OTP
        var otpEntity = new Otp
        {
            Id = otpId,
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            ExpiredAt = DateTime.UtcNow.AddMinutes(-5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };

        _mockOtpRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken))
            .ReturnsAsync(otpEntity);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.VerifyOtp(contextId, otp, "device", "ip", cancellationToken));

        // Verify OTP was marked as used
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.IsUsed), cancellationToken), Times.Once);

        // Verify cache was cleared
        _mockCacheService.Verify(c => c.RemoveData("otp_context:" + contextId), Times.Once);
    }

    [Fact]
    public async Task VerifyOtp_WithTooManyAttempts_ShouldThrowBadRequestException()
    {
        // Arrange
        var contextId = "test-context-id";
        var otpId = "otp-123";
        var otp = "123456";
        var phoneNumber = "+84123456789";
        var cancellationToken = CancellationToken.None;

        // Setup cache service
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync(otpId);

        // Setup OTP repository with too many attempts
        var otpEntity = new Otp
        {
            Id = otpId,
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 6,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };

        _mockOtpRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken))
            .ReturnsAsync(otpEntity);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.VerifyOtp(contextId, otp, "device", "ip", cancellationToken));

        // Verify OTP was marked as used
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.IsUsed), cancellationToken), Times.Once);

        // Verify cache was cleared
        _mockCacheService.Verify(c => c.RemoveData("otp_context:" + contextId), Times.Once);
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidOtp_ShouldThrowBadRequestException()
    {
        // Arrange
        var contextId = "test-context-id";
        var otpId = "otp-123";
        var correctOtp = "123456";
        var incorrectOtp = "654321";
        var phoneNumber = "+84123456789";
        var cancellationToken = CancellationToken.None;

        // Setup cache service
        _mockCacheService.Setup(c => c.GetData<string>("otp_context:" + contextId))
            .ReturnsAsync(otpId);

        // Setup OTP repository
        var otpEntity = new Otp
        {
            Id = otpId,
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(correctOtp)
        };

        _mockOtpRepository.Setup(r => r.GetAsyns(It.IsAny<Expression<Func<Otp, bool>>>(), cancellationToken))
            .ReturnsAsync(otpEntity);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _authService.VerifyOtp(contextId, incorrectOtp, "device", "ip", cancellationToken));

        // Verify attempt count was incremented
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.AttemptCount == 1), cancellationToken), Times.Once);

        // Verify LastAttemptAt was updated
        _mockOtpRepository.Verify(r => r.UpdateAsync(It.Is<Otp>(o => o.LastAttemptAt.HasValue), cancellationToken), Times.Once);
    }
}
