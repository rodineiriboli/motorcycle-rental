using MR.DeliveryOrder.Enums;

namespace MR.DeliveryOrder.Models;

public class BaseDomain
{
    public Guid Id { get; set; }
    public Guid ChangeUserId { get; set; }
    public DateTime IncludeDate { get; set; }
    public DateTime ChangeDate { get; set; }
    public StatusValue Status { get; set; }

    protected BaseDomain(StatusValue statusValue = StatusValue.Active)
    {
        Id = Guid.NewGuid();
        IncludeDate = DateTime.UtcNow;
        Status = statusValue;
    }
}
