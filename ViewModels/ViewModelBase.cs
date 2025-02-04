using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPF_Tool_MultiFolderCreator.ViewModels
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        protected ViewModelBase(IMessenger messenger) : base(messenger)
        {
            // Optional: Automatisch für Nachrichten registrieren
            IsActive = true;
        }
        public virtual void Initialize() { }
    }
}
