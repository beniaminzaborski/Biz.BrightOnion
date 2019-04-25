using Biz.BrightOnion.Catalog.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Entities
{
  public class Room : Entity
  {
    [Required]
    [MaxLength(25)]
    public virtual string Name { get; set; }

    public virtual long? ManagerId { get; set; }

    [MaxLength(250)]
    public virtual string ManagerName { get; set; }

    public virtual int SlicesPerPizza { get; set; } = 8;
  }
}
