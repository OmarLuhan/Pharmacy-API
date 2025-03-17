using Farma_api.Dto.Grn;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class GrnValidator : AbstractValidator<GrnCreateDto>
{
    public GrnValidator()
    {
        RuleFor(x => x.UsuarioId).NotNull();
        RuleFor(x => x.Documento)
            .NotEmpty()
            .Must(value => value is "Boleta" or "Factura");
        RuleFor(x => x.Proveedor).MaximumLength(30);
        RuleFor(x => x.DetalleEntrada).NotNull();
        RuleForEach(x => x.DetalleEntrada).SetValidator(new GrnDetailValidator());
    }
}

public class GrnDetailValidator : AbstractValidator<GrnDetailCreateDto>
{
    public GrnDetailValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.LoteId).GreaterThan(0);
    }
}