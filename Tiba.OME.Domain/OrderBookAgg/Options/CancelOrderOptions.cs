using Tiba.OME.Domain.Core;

namespace Tiba.OME.Domain.OrderBookAgg.Options;

public class CancelOrderOptions(Guid OrderId) : IDomainCommand;

