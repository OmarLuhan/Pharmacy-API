using Farma_api.Dto.Batch;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class BatchUpdateValidator : AbstractValidator<BatchUpdateDto>
{
    public BatchUpdateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NumeroLote).NotEmpty().MaximumLength(30);
        RuleFor(x => x.StockLote).GreaterThan(-1);
        RuleFor(x => x.Activo).NotNull();
        RuleFor(x => x.FechaProduccion).NotEmpty();
        RuleFor(x => x.FechaVencimiento).NotEmpty();
    }
}

public class BatchCreateValidator : AbstractValidator<BatchCreateDto>
{
    public BatchCreateValidator()
    {
        RuleFor(x => x.NumeroLote).NotEmpty().MaximumLength(30);
        RuleFor(x => x.ProductoId).GreaterThan(0);
        RuleFor(x => x.FechaProduccion).NotNull();
        RuleFor(x => x.FechaVencimiento).NotNull();
        RuleFor(x => x)
            .Must(dto => dto.FechaVencimiento > dto.FechaProduccion)
            .WithMessage("La fecha de vencimiento debe ser posterior a la fecha de producción.");
    }
}