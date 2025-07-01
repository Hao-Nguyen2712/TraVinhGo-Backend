// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpCompress.Common;
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

    public ReviewService(IBaseRepository<Domain.Entities.Review> baseRepository, ImageManagementReviewServices imageManagementReviewServices, IHttpContextAccessor httpContextAccessor, IUserService userService, IReviewRepository reviewRepository)
    {
        _baseRepository = baseRepository;
        _imageManagementReviewServices = imageManagementReviewServices;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _reviewRepository = reviewRepository;
    }

    public async Task<Domain.Entities.Review> AddAsync(CreateReviewRequest createReviewRequest, CancellationToken cancellationToken = default)
    {
        if (createReviewRequest == null)
            throw new ArgumentNullException(nameof(createReviewRequest));

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var review = ReviewMapper.Mapper.Map<CreateReviewRequest, Domain.Entities.Review>(createReviewRequest);
        review.UserId = userId;

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

    public async Task<IEnumerable<Domain.Entities.Review>> GetReviewsAsync(int rating, string destinationTypeId, CancellationToken cancellationToken = default)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        return await _reviewRepository.GetReviewsAsync(rating, destinationTypeId, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.Review>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _reviewRepository.ListAllAsync(cancellationToken);
    }
}
