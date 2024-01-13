namespace Tiba.OME.Domain.OrderBookAgg.Exceptions;

public class InstrumentNotMatchException() : BusinessException(ErrorMessage)
{
    public const string ErrorMessage = "نماد مطابقت ندارد.";
}