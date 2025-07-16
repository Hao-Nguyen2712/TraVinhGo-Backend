// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Http;
using SharpCompress.Common;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.Feedback;
using TraVinhMaps.Application.Features.Feedback.Mapper;
using TraVinhMaps.Application.Features.Feedback.Models;
using TraVinhMaps.Application.Features.Review.Interface;
using TraVinhMaps.Application.Features.Review.Mappers;
using TraVinhMaps.Application.Features.Review.Models;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Review;
public class ReviewService : IReviewService
{
    private readonly IBaseRepository<Domain.Entities.Review> _baseRepository;
    private readonly ImageManagementReviewServices _imageManagementReviewServices;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IReviewRepository _reviewRepository;
    private readonly ITouristDestinationService _touristDestinationService;

    public ReviewService(IBaseRepository<Domain.Entities.Review> baseRepository, ImageManagementReviewServices imageManagementReviewServices, IHttpContextAccessor httpContextAccessor, IUserService userService, IReviewRepository reviewRepository, ITouristDestinationService touristDestinationService)
    {
        _baseRepository = baseRepository;
        _imageManagementReviewServices = imageManagementReviewServices;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _reviewRepository = reviewRepository;
        _touristDestinationService = touristDestinationService;
    }

    public async Task<Domain.Entities.Review> AddAsync(CreateReviewRequest createReviewRequest, List<string> imageUrl, CancellationToken cancellationToken = default)
    {
        if (createReviewRequest == null)
            throw new ArgumentNullException(nameof(createReviewRequest));

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var review = ReviewMapper.Mapper.Map<CreateReviewRequest, Domain.Entities.Review>(createReviewRequest);
        review.UserId = userId;
        review.Images = imageUrl;

        if (createReviewRequest.Images != null && createReviewRequest.Images.Any())
        {
            review.Images = await _imageManagementReviewServices.AddImageReview(createReviewRequest.Images);
        }
        else
        {
            review.Images = new List<string>();
        }

        return await _baseRepository.AddAsync(review, cancellationToken);
    }

    public Task<string> AddImageReview(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return _reviewRepository.AddImageReview(id, imageUrl, cancellationToken);
    }

    public async Task<Reply> AddReply(string id, CreateReplyRequest createReplyRequest, CancellationToken cancellationToken = default)
    {
        if (createReplyRequest == null)
            throw new ArgumentNullException(nameof(createReplyRequest));

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var reply = ReviewMapper.Mapper.Map<CreateReplyRequest, Domain.Entities.Reply>(createReplyRequest);
        reply.UserId = userId;

        if (createReplyRequest.Images != null && createReplyRequest.Images.Any())
        {
            reply.Images = await _imageManagementReviewServices.AddImageReview(createReplyRequest.Images);
        }
        else
        {
            reply.Images = new List<string>();
        }

        return await _reviewRepository.AddReply(id, reply, cancellationToken);
    }


    public Task<long> CountAsync(Expression<Func<Domain.Entities.Review, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _reviewRepository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(Domain.Entities.Review entity, CancellationToken cancellationToken = default)
    {
        return _reviewRepository.DeleteAsync(entity, cancellationToken);
    }

    public Task<Domain.Entities.Review> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _reviewRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<ReviewResponse>> FilterReviewsAsync(string? destinationId, int? rating, DateTime? startAt, DateTime? endAt, CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewRepository.FilterReviewsAsync(destinationId, rating, startAt, endAt, cancellationToken);
        var reviewResponses = new List<ReviewResponse>();

        foreach (var reviewResponse in reviews)
        {
            var user = await _userService.GetByIdAsync(reviewResponse.UserId);
            var destination = await _touristDestinationService.GetByIdAsync(reviewResponse.DestinationId);

            var response = new ReviewResponse
            {
                Id = reviewResponse.Id,
                Rating = reviewResponse.Rating,
                Images = reviewResponse.Images,
                Comment = reviewResponse.Comment,
                UserId = reviewResponse.UserId,
                Avatar = user?.Profile?.Avatar ?? "Unknown",
                UserName = user?.Username ?? "Unknown",
                DestinationId = reviewResponse.DestinationId,
                DestinationName = destination?.Name ?? "Unknown",
                CreatedAt = reviewResponse.CreatedAt,
                Reply = reviewResponse.Reply?.Select(r => new Reply
                {
                    Content = r.Content,
                    Images = r.Images,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                }).ToList(),
            };
            reviewResponses.Add(response);
        }
        return reviewResponses;
    }

    public async Task<IEnumerable<ReviewResponse>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var review = await _baseRepository.ListAllAsync(cancellationToken);
        var reviewResponses = new List<ReviewResponse>();

        foreach (var reviewResponse in review)
        {
            var user = await _userService.GetByIdAsync(reviewResponse.UserId);
            var destination = await _touristDestinationService.GetByIdAsync(reviewResponse.DestinationId);

            var response = new ReviewResponse
            {
                Id = reviewResponse.Id,
                Rating = reviewResponse.Rating,
                Images = reviewResponse.Images,
                Comment = reviewResponse.Comment,
                UserId = reviewResponse.UserId,
                UserName = user?.Username ?? "Unknown",
                DestinationId = reviewResponse.DestinationId,
                DestinationName = destination?.Name ?? "Unknown",
                CreatedAt = reviewResponse.CreatedAt,
                Reply = reviewResponse.Reply?.Select(r => new Reply
                {
                    Content = r.Content,
                    Images = r.Images,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                }).ToList(),
            };
            reviewResponses.Add(response);
        }
        return reviewResponses;
    }

    public async Task<ReviewResponse> GetReviewByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(id);
        var user = await _userService.GetByIdAsync(review.UserId);
        var destination = await _touristDestinationService.GetByIdAsync(review.DestinationId);

        var response = new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Images = review.Images,
            Comment = review.Comment,
            UserId = review.UserId,
            Avatar = user?.Profile?.Avatar ?? "Unknown",
            UserName = user?.Username ?? "Unknown",
            DestinationId = review.DestinationId,
            DestinationName = destination?.Name ?? "Unknown",
            CreatedAt = review.CreatedAt,
            Reply = review.Reply?.Select(r => new Reply
            {
                Content = r.Content,
                Images = r.Images,
                CreatedAt = r.CreatedAt,
                UserId = r.UserId
            }).ToList()
        };
        return response;
    }

    public async Task<IEnumerable<ReviewResponse>> GetLatestReviewsAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewRepository.GetLatestReviewsAsync(count, cancellationToken);
        var reviewResponses = new List<ReviewResponse>();

        foreach (var reviewResponse in reviews)
        {
            var user = await _userService.GetByIdAsync(reviewResponse.UserId);
            var destination = await _touristDestinationService.GetByIdAsync(reviewResponse.DestinationId);

            var response = new ReviewResponse
            {
                Id = reviewResponse.Id,
                Rating = reviewResponse.Rating,
                Images = reviewResponse.Images,
                Comment = reviewResponse.Comment,
                UserId = reviewResponse.UserId,
                UserName = user?.Username ?? "Unknown",
                DestinationId = reviewResponse.DestinationId,
                DestinationName = destination?.Name ?? "Unknown",
                CreatedAt = reviewResponse.CreatedAt,
                Reply = reviewResponse.Reply?.Select(r => new Reply
                {
                    Content = r.Content,
                    Images = r.Images,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                }).ToList(),
            };
            reviewResponses.Add(response);
        }
        return reviewResponses;
    }
    public async Task<ReviewMobileListResponse> FilterReviewsMobileAsync(
    string? destinationId,
    int? rating,
    DateTime? startAt,
    DateTime? endAt,
    CancellationToken cancellationToken = default)
    {
        bool hasReviewed = false;
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            hasReviewed = false;

        var reviews = await _reviewRepository.FilterReviewsAsync(destinationId, rating, startAt, endAt, cancellationToken);

        var reviewResponses = new List<ReviewMobileResponse>();
        var ratingSummary = new RatingSummaryResponse();

        foreach (var review in reviews)
        {
            // Đếm số lượng sao
            switch (review.Rating)
            {
                case 1: ratingSummary.OneStar++; break;
                case 2: ratingSummary.TwoStar++; break;
                case 3: ratingSummary.ThreeStar++; break;
                case 4: ratingSummary.FourStar++; break;
                case 5: ratingSummary.FiveStar++; break;
            }

            // Lấy thông tin người dùng viết review
            var user = await _userService.GetByIdAsync(review.UserId);
            var (userFullName, userAvatar) = ExtractUserInfo(user);

            var replyResponses = new List<RelyResponse>();

            if (review.Reply != null && review.Reply.Any())
            {
                foreach (var reply in review.Reply)
                {
                    var replyUser = await _userService.GetByIdAsync(reply.UserId);
                    var (replyFullName, replyAvatar) = ExtractUserInfo(replyUser);

                    replyResponses.Add(new RelyResponse
                    {
                        Content = reply.Content,
                        Images = reply.Images,
                        UserId = reply.UserId,
                        UserName = replyFullName,
                        Avatar = replyAvatar,
                        CreatedAt = reply.CreatedAt
                    });
                }
            }

            reviewResponses.Add(new ReviewMobileResponse
            {
                Id = review.Id,
                Rating = review.Rating,
                Images = review.Images,
                Comment = review.Comment,
                UserId = review.UserId,
                Avatar = userAvatar,
                UserName = userFullName,
                DestinationId = review.DestinationId,
                CreatedAt = review.CreatedAt,
                Reply = replyResponses
            });
        }

        int totalRatings = ratingSummary.OneStar + ratingSummary.TwoStar + ratingSummary.ThreeStar +
                   ratingSummary.FourStar + ratingSummary.FiveStar;

        if (totalRatings > 0)
        {
            ratingSummary.OneStarPercent = Math.Round((ratingSummary.OneStar * 100.0) / totalRatings, 2);
            ratingSummary.TwoStarPercent = Math.Round((ratingSummary.TwoStar * 100.0) / totalRatings, 2);
            ratingSummary.ThreeStarPercent = Math.Round((ratingSummary.ThreeStar * 100.0) / totalRatings, 2);
            ratingSummary.FourStarPercent = Math.Round((ratingSummary.FourStar * 100.0) / totalRatings, 2);
            ratingSummary.FiveStarPercent = Math.Round((ratingSummary.FiveStar * 100.0) / totalRatings, 2);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            hasReviewed = !reviewResponses.Any(item => item.UserId == userId);
        }

        return new ReviewMobileListResponse
        {
            hasReviewed = hasReviewed,
            Reviews = reviewResponses,
            RatingSummary = ratingSummary
        };
    }

    private (string fullName, string? avatar) ExtractUserInfo(User? user)
    {
        if (user?.Profile != null)
        {
            return (
                string.IsNullOrWhiteSpace(user.Profile.FullName) ? "Unknown" : user.Profile.FullName,
                user.Profile.Avatar
            );
        }

        return ("Unknown", null);
    }

    public async Task<ReviewMobileResponse> AddMobileAsync(CreateReviewRequest createReviewRequest, List<string> imageUrl, CancellationToken cancellationToken = default)
    {
        if (createReviewRequest == null)
            throw new ArgumentNullException(nameof(createReviewRequest));

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var review = ReviewMapper.Mapper.Map<CreateReviewRequest, Domain.Entities.Review>(createReviewRequest);
        review.UserId = userId;

        // Xử lý ảnh
        if (createReviewRequest.Images != null && createReviewRequest.Images.Any())
        {
            review.Images = await _imageManagementReviewServices.AddImageReview(createReviewRequest.Images);
        }
        else
        {
            review.Images = imageUrl ?? new List<string>();
        }

        // Lưu vào DB
        var addedReview = await _baseRepository.AddAsync(review, cancellationToken);

        var reviews = await _reviewRepository.FilterReviewsAsync(addedReview.DestinationId, null, null, null, cancellationToken);
        var average = reviews.Average(r => r.Rating);
        await _touristDestinationService.UpdateAverageRatingAsync(addedReview.DestinationId, average);

        // Lấy thông tin người dùng
        var user = await _userService.GetByIdAsync(userId);
        var (userFullName, userAvatar) = ExtractUserInfo(user);

        // Map replies nếu có (thường mới thêm sẽ chưa có reply)
        var replyResponses = new List<RelyResponse>();
        if (addedReview.Reply != null && addedReview.Reply.Any())
        {
            foreach (var reply in addedReview.Reply)
            {
                var replyUser = await _userService.GetByIdAsync(reply.UserId);
                var (replyFullName, replyAvatar) = ExtractUserInfo(replyUser);

                replyResponses.Add(new RelyResponse
                {
                    Content = reply.Content,
                    Images = reply.Images,
                    UserId = reply.UserId,
                    UserName = replyFullName,
                    Avatar = replyAvatar,
                    CreatedAt = reply.CreatedAt
                });
            }
        }

        return new ReviewMobileResponse
        {
            Id = addedReview.Id,
            Rating = addedReview.Rating,
            Images = addedReview.Images,
            Comment = addedReview.Comment,
            UserId = addedReview.UserId,
            Avatar = userAvatar,
            UserName = userFullName,
            DestinationId = addedReview.DestinationId,
            CreatedAt = addedReview.CreatedAt,
            Reply = replyResponses
        };
    }

    public async Task<RelyResponse> AddMobileReply(string id, CreateReplyRequest createReplyRequest, CancellationToken cancellationToken = default)
    {
        if (createReplyRequest == null)
            throw new ArgumentNullException(nameof(createReplyRequest));

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var reply = ReviewMapper.Mapper.Map<CreateReplyRequest, Domain.Entities.Reply>(createReplyRequest);
        reply.UserId = userId;

        // Xử lý ảnh
        if (createReplyRequest.Images != null && createReplyRequest.Images.Any())
        {
            reply.Images = await _imageManagementReviewServices.AddImageReview(createReplyRequest.Images);
        }
        else
        {
            reply.Images = new List<string>();
        }

        // Lưu vào repository
        var addedReply = await _reviewRepository.AddReply(id, reply, cancellationToken);

        // Lấy thông tin người dùng
        var user = await _userService.GetByIdAsync(userId);
        var (userFullName, userAvatar) = ExtractUserInfo(user);

        // Trả về dữ liệu phản hồi chi tiết
        return new RelyResponse
        {
            Content = addedReply.Content,
            Images = addedReply.Images,
            CreatedAt = addedReply.CreatedAt,
            UserId = addedReply.UserId,
            UserName = userFullName,
            Avatar = userAvatar
        };
    }

    public Task<long> GetTotalUsersReviewedAsync(CancellationToken cancellationToken = default)
    {
        return _reviewRepository.GetTotalUsersReviewedAsync(cancellationToken);
    }

    public Task<long> GetTotalFiveStarReviewsAsync(CancellationToken cancellationToken = default)
    {
        return _reviewRepository.GetTotalFiveStarReviewsAsync(cancellationToken);
    }

    public Task<(string UserId, long ReviewCount)> GetTopReviewerAsync(CancellationToken cancellationToken = default)
    {
        return _reviewRepository.GetTopReviewerAsync(cancellationToken);
    }
}
