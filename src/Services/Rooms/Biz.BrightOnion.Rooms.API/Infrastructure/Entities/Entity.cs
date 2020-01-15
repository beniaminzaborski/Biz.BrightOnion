using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Rooms.API.Infrastructure.Entities
{
  public abstract class Entity
  {
    public virtual long Id { get; set; }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Entity other = obj as Entity;
      if (other != null)
        return Id.Equals(other.Id);
      return base.Equals(obj);
    }

    public static bool operator ==(Entity x, Entity y)
    {
      return Equals(x, y);
    }

    public static bool operator !=(Entity x, Entity y)
    {
      return !(x == y);
    }
  }
}
