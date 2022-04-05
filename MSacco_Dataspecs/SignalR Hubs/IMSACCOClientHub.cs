using MSacco_Dataspecs.Security;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.SignalR_Hubs
{
    public interface IMSACCOClientHub
    {
        /// <summary>
        /// Client sends the uploaded filestream through this function
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="message"></param>
        /// <param name="commParams"></param>
        void Onyesha(string userID, string message, IMSACCO_AES_Credentials commParams);

    }

    public interface IMSACCOClientApp
    {
        /// <summary>
        /// Client implements a function with this name for purposes of
        /// receiving notification updates from the server
        /// </summary>
        /// <param name="communicationParamms"></param>
        /// <param name="message"></param>
        void serverMessage(string communicationParamms, string message);
    }
}