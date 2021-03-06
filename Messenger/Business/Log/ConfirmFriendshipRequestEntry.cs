﻿namespace Business.Log
{
    public class ConfirmFriendshipRequestEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string ConfirmerUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {ConfirmerUsername} confirmed a friendship request from {SenderUsername}.";
        }
    }
}
