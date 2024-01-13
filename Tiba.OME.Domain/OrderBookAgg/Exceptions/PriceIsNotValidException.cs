namespace Tiba.OME.Domain.OrderBookAgg.Exceptions;

public class PriceIsNotValidException() : BusinessException(ErrorMessage)
{
    public const string ErrorMessage = "قیمت مورد نظر باید بزرگتر از صفر باشد.";
}