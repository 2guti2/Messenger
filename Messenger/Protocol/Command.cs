﻿namespace Protocol
{
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
    }
}