using MSacco_Dataspecs.Security;

namespace MSacco_Dataspecs.SignalR_Hubs
{
    public interface IMSACCOClientHub
    {
        void Onyesha(string userID, string message, IMSACCO_AES_Credentials commParams);

        string GetClientID();
    }

    public interface IMSACCOClientApp
    {
        void serverMessage(string communicationParamms, string message);
    }
}