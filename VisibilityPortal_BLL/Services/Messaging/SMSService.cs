﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace VisibilityPortal_BLL.Services.Messaging
{
  public class SmsService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message)
    {
      // Plug in your SMS service here to send a text message.
      return Task.FromResult(0);
    }
  }
}
