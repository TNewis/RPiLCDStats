using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using RPiLCDPiServer.Services.ConnectionService;

namespace RPiLCDPiServer.ViewModels
{
    public abstract class UpdateableViewModelBase : ReactiveObject
    {
        private string _updateString;
        public string UpdateString
        {
            get { return _updateString; }
            set { this.RaiseAndSetIfChanged(ref _updateString, value); }
        }
        protected void HandleUpdateRecievedEvent(object sender, UpdateRecievedEventArgs a)
        {
            ProcessUpdate(a.UpdateString);
        }

        protected abstract void ProcessUpdate(string message);

    }
}
