using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Dto
{
  public class RoomDTO
  {
    public long Id { get; set; }
    public string Name { get; set; }
    //public long? ManagerId { get; set; }
    //public string ManagerName { get; set; }
    public UserDTO Manager { get; set; }
    public int SlicesPerPizza { get; set; } = 8;
  }

  public class UserDTO
  {
    public long? UserId { get; set; }
    public string Email { get; set; }
  }
}
