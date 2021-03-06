﻿using RPiLCDShared.Services;
using System;

namespace RPiLCDPiServer.Services.ConnectionService
{
    public abstract class ConnectionService: IConnectionService
    {
        protected static UpdateRecievedEventArgs _args = new UpdateRecievedEventArgs();
        public static event EventHandler<UpdateRecievedEventArgs> RaiseUpdateRecievedEvent;

        protected static void OnUpdateRecieved(UpdateRecievedEventArgs e)
        {
            RaiseUpdateRecievedEvent?.Invoke(null, e); ;
        }

        public abstract void SetLoggingService(ILoggingService loggingService);
        public abstract void CloseConnection();
        public abstract void OpenConnection();
        public abstract void IncludeInResponse(string response);
    }

    public class UpdateRecievedEventArgs : EventArgs
    {
        public string UpdateString { get; set; }
    }
}
