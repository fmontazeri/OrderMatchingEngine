namespace Tiba.OME.Application.Exceptions;

public class OrderBookNotFound() : ApplicationException(ErrorMessage)
{
    public const string ErrorMessage = "امکان ثبت درخواست خرید یا فروش برای این نماد وجود ندارد.";
}