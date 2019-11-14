﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.ApiGateway.OrderingAggregator.Infrastructure
{
  public class InternalServerErrorObjectResult : ObjectResult
  {
    public InternalServerErrorObjectResult(object error)
        : base(error)
    {
      StatusCode = StatusCodes.Status500InternalServerError;
    }
  }
}
