namespace Tiba.OME.Domain.OrderBookAgg.Exceptions;

public class OrderNofFoundException() : BusinessException(ErrorMessage)
{
    public const string ErrorMessage = "Order not found!";
}