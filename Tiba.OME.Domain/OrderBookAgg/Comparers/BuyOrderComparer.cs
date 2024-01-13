namespace Tiba.OME.Domain.OrderBookAgg.Comparers;

public class BuyOrderComparer : IComparer<IOrder>
{
    public int Compare(IOrder? order1, IOrder? order2)
    {
        // Return a positive value if o2 should be dequeued before o1
        if (order1.Price < order2.Price) return 1;
        // Return a negative value if o1 should be dequeued before o2
        if (order1.Price > order2.Price) return -1;
        if (order1.CreatedOn < order2.CreatedOn) return -1;
        return 1;
    }
}

