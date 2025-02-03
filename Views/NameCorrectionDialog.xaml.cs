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


namespace WPF_Tool_MultiFolderCreator.Views
{
    /// <summary>
    /// Interaktionslogik für NameCorrectionDialog.xaml
    /// </summary>

    public partial class NameCorrectionDialog : Window
    {
        public string CorrectedName { get; private set; } = string.Empty;
        public string SuggestedName
        {
            set
            {
                tb_CorrectedName.Text = value;
            }
        }

        public NameCorrectionDialog(string originalName)
        {
            InitializeComponent();
            OriginalNameText.Text = originalName;
            tb_CorrectedName.Text = originalName;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_CorrectedName.Text))
            {
                MessageBox.Show("Bitte geben Sie einen Namen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CorrectedName = tb_CorrectedName.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
    }