using System;

namespace Business
{
    [Serializable]
    public enum Command
    {
        FriendshipRequest,
        Login,
        GetFriendshipRequests,
        ConfirmFriendshipRequest,
        RejectFriendshipRequest,
        DisconnectUser,
        ListMyFriends,
        ListOfConnectedUsers,
        ListOfAllClients,
        SendMessage,
        ReadMessage,
        GetConversation,
        UploadFile,
        ListClientFiles,
        DownloadFile
    }
}