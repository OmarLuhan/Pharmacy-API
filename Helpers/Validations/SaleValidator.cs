using Farma_api.Dto.Sales;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class SaleCreateValidator : AbstractValidator<SaleCreateDto>
{
    public SaleCreateValidator()
    {
        RuleFor(x => x.UsuarioId).NotNull();
        RuleFor(x => x.ClienteDni).MaximumLength(12);
        RuleFor(x => x.ClienteNombre).MaximumLength(30);
        RuleFor(x => x.DetalleVenta).NotNull();
        RuleForEach(x => x.DetalleVenta).SetValidator(new SaleDetailValidator());
    }
}

public class SaleDetailValidator : AbstractValidator<SaleDetailCreateDto>
{
    public SaleDetailValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
    }
}