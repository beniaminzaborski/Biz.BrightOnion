using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.BrightOnion.Ordering.Domain.Seedwork
{
  public abstract class Entity
  {
    int? requestedHashCode;
    long id;
    public virtual long Id
    {
      get
      {
        return id;
      }
      protected set
      {
        id = value;
      }
    }

    private List<INotification> domainEvents;
    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
      domainEvents = domainEvents ?? new List<INotification>();
      domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
      domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
      domainEvents?.Clear();
    }

    public bool IsTransient()
    {
      return this.Id == default(Int64);
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is Entity))
        return false;

      if (Object.ReferenceEquals(this, obj))
        return true;

      if (this.GetType() != obj.GetType())
        return false;

      Entity item = (Entity)obj;

      if (item.IsTransient() || this.IsTransient())
        return false;
      else
        return item.Id == this.Id;
    }

    public override int GetHashCode()
    {
      if (!IsTransient())
      {
        if (!requestedHashCode.HasValue)
          requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

        return requestedHashCode.Value;
      }
      else
        return base.GetHashCode();

    }
    public static bool operator ==(Entity left, Entity right)
    {
      if (Object.Equals(left, null))
        return (Object.Equals(right, null)) ? true : false;
      else
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
      return !(left == right);
    }
  }
}
