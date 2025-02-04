using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_Tool_MultiFolderCreator.ViewModels
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        public virtual void Initialize() { }
    }
}
