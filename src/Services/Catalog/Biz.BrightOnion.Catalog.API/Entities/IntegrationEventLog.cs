using Biz.BrightOnion.Catalog.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Entities
{
  public class IntegrationEventLog : Entity
  {
    [Required]
    public virtual Guid EventId { get; set; }

    [Required]
    public virtual DateTime EventCreationDate { get; set; }

    [Required]
    public virtual string EventType { get; set; }

    [Required]
    public virtual string EventBody { get; set; }

    [Required]
    public virtual IntegrationEventState State { get; set; }
  }

  public enum IntegrationEventState
  {
    ReadyToPublish,
    Published
  }
}
