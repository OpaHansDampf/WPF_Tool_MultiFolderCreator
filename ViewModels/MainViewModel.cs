using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPF_Tool_MultiFolderCreator.Models;
using Microsoft.Win32;
using System.IO;

namespace WPF_Tool_MultiFolderCreator.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string csvPath = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private string targetPath = string.Empty;
        

        [RelayCommand]
        private void SelectCsvFile()
        {
            // Wir erstellen den FileDialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
            };

            // Wenn eine Datei ausgewählt wurde
            if (openFileDialog.ShowDialog() == true)
            {
                // Setzen der Properties - diese aktualisieren automatisch die UI
                CsvPath = openFileDialog.FileName;
                StatusMessage = $"CSV-Datei ausgewählt: {CsvPath}";
            }
        }

        [RelayCommand]
        private void SelectTragetFolder()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Zielordner wählen",
                FileName = "Ordner auswählen"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TargetPath = Path.GetDirectoryName(saveFileDialog.FileName) ?? string.Empty;
                StatusMessage = $"Zielordner ausgewählt: {TargetPath}";
            }
        }

        public MainViewModel()
        {
            Initialize();
        }

        public override void Initialize()
        {
            //base.Initialize();
            // Sample Data Input //

        }
    }
}