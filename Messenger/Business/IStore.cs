﻿namespace Business
{
    public interface IStore
    {
        bool ClientExists(Client client);
        void AddClient(Client client);
    }
}