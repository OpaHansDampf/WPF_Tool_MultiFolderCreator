using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_Tool_MultiFolderCreator.Models
{
    public partial class SamplePerson : ObservableObject
    {
        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public int age;

        public SamplePerson(string name, int age)
        {
            this.name = name;
            this.age = age;
        }
    }
}
