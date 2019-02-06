using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.Entities
{
  public class User : Entity
  {
    [Required]
    [StringLength(250)]
    public string Email { get; set; }

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; }

    public bool NotificationEnabled { get; set; }
  }
}
