using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MSacco_Dataspecs.Security;
using MSacco_Dataspecs.SignalR_Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MSacco_BLL.Hubs
{
    [HubName("msaccoClient")]
    //public class MSACCOClientHub : Hub, IMSACCOClientHub
    public class MSACCOClientHub : Hub<IMSACCOClientApp>, IMSACCOClientHub
    {
        public override Task OnConnected()
        {
            string version = Context.ConnectionId;

            if (Context.User != null)
            {
                AppLogger.LogDevNotes("MSACCOClientHub.OnConnected", "Connected to client hub", new { user = Context.User.Identity.Name });
            }
            else
            {
                AppLogger.LogDevNotes("MSACCOClientHub.OnConnected", "Connected to client hub without user in Context", null);

                if(!string.IsNullOrEmpty(Context.Request.User.Identity.Name))
                {
                    AppLogger.LogDevNotes("MSACCOClientHub.OnConnected", "but Context.Request.User is not null", new { Context.Request.User.Identity.Name });
                }
            }


            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        //public string GetClientID()
        //{
        //    return Context.ConnectionId;
        //}

        public void Onyesha(string userID, string message, IMSACCO_AES_Credentials commParams)
        {
            AppLogger.LogDevNotes("MSACCOClientHub.Onyesha", "Sending server response to user", new { message, userID, commParams });
            Clients.User(userID)?.serverMessage(
                JsonConvert.SerializeObject(new
                {
                    encSecret = commParams.CipherKey,
                    encKey = commParams.CipherIV
                }),
            message);
        }



    }
}
