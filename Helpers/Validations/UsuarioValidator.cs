using Farma_api.Dto.Profile;
using Farma_api.Dto.User;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class ProfileUpdateValidator : AbstractValidator<ProfileUpdateDto>
{
    public ProfileUpdateValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Nombre).NotNull().MaximumLength(30);
        RuleFor(x => x.Correo).EmailAddress().NotNull();
    }
}

public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Nombre).NotNull().MaximumLength(30);
        RuleFor(x => x.Correo).EmailAddress().NotNull();
        RuleFor(x => x.RolId).GreaterThan(0);
        RuleFor(x => x.Activo).NotNull();
    }
}

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(x => x.Nombre).NotNull().MaximumLength(30);
        RuleFor(x => x.Correo).EmailAddress().NotNull();
        RuleFor(x => x.RolId).GreaterThan(0);
    }
}