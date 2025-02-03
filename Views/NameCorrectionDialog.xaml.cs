using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Tool_MultiFolderCreator.ViewModels;

namespace WPF_Tool_MultiFolderCreator.Views
{
    public partial class NameCorrectionDialog : Window
    {
        private readonly NameCorrectionViewModel _viewModel;

        public string CorrectedName => _viewModel.CorrectedName;

        public NameCorrectionDialog(NameCorrectionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
