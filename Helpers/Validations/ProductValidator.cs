using Farma_api.Dto.Product;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CodigoEan13).Length(4, 14).WithMessage("El código debe tener entre 6 y 13 caracteres");
        ;
        RuleFor(x => x.Nombre).Length(5, 50);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.Presentacion).MaximumLength(50);
        RuleFor(x => x.Concentracion).MaximumLength(20);
        RuleFor(x => x.Precio).GreaterThan(-1);
        RuleFor(x => x.Especial).NotNull();
    }
}

public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.CodigoEan13).Length(4, 14).WithMessage("El código debe tener entre 6 y 13 caracteres.");
        RuleFor(x => x.Nombre).Length(5, 50);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.Presentacion).MaximumLength(50);
        RuleFor(x => x.Concentracion).MaximumLength(20);
        RuleFor(x => x.Precio).GreaterThan(-1);
        RuleFor(x => x.Especial).NotNull();
        RuleForEach(x => x.Lotes).SetValidator(new ProductBatchCreateValidator());
    }
}

public class ProductBatchCreateValidator : AbstractValidator<ProductBatchCreateDto>
{
    public ProductBatchCreateValidator()
    {
        RuleFor(x => x.NumeroLote).NotNull().MaximumLength(30);
        RuleFor(x => x.FechaProduccion).NotEmpty();
        RuleFor(x => x.FechaVencimiento).NotEmpty();
    }
}