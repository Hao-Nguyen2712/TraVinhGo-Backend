// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using FluentValidation;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
        .NotEmpty().WithMessage("Username is required.")
        .MaximumLength(50);

        RuleFor(user => user.PhoneNumber)
        .NotEmpty().WithMessage("Phone number is required.")
        .Matches(@"^\+?[1-9]\d{1,14}$").When(user => !string.IsNullOrEmpty(user.PhoneNumber))
        .WithMessage("Phone number must be a valid international format (e.g., +1234567890).");

        RuleFor(user => user.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(user => user.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Email is required.");

        RuleFor(user => user.RoleId)
        .NotEmpty().WithMessage("RoleId is required.");

        //  RuleFor(user => user.Profile)
        // .SetValidator(new ProfileValidator()!).When(user => user.Profile != null);

        RuleFor(user => user.Favorites)
        .ForEach(favorite => favorite.Must(f => f != null))
        .WithMessage("Favorites list cannot contain null entries.")
        .When(user => user.Favorites != null && user.Favorites.Any());
    }
}
