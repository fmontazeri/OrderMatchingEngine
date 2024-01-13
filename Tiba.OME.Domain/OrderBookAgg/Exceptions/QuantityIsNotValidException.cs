namespace Tiba.OME.Domain.OrderBookAgg.Exceptions;

public class QuantityIsNotValidException() : BusinessException(ErrorMessage)
{
    public const string ErrorMessage = "حجم وارد شده باید بزرگتر از صفر باشد.";
}