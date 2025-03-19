using FluentValidation;
using WebApiTemplate.DTO;
using System.Text.RegularExpressions;

namespace WebApiTemplate.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .Matches(@"^[a-zA-Z0-9._%+-]+@gmail\.com$")
                .WithMessage("Only Gmail addresses are allowed");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
}