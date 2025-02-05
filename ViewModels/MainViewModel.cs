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
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using WPF_Tool_MultiFolderCreator.Services.Logging;
using System.Windows.Media;

namespace WPF_Tool_MultiFolderCreator.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IMessenger _messenger;
        private readonly LoggingService _loggingService;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private string csvPath = string.Empty;

        [ObservableProperty]
        private string targetPath = string.Empty;

        [ObservableProperty]
        private int createdFolders;

        [ObservableProperty]
        private int createdSubFolders;

        [ObservableProperty]
        private int existingFolders;

        [ObservableProperty]
        private int existingSubFolders;

        [ObservableProperty]
        private int correctedNames;

        [RelayCommand]
        private void SelectCsvFile() // old: CsvButton_Click
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
                //tblock_SelectedCsvFilePath ??????????????????????????
                SendLogMessage(LogEntryType.CsvSelected, CsvPath);
            }
        }

        [RelayCommand]
        private void SelectTragetFolder() // old: FolderButton_Click //Log einbauen
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Zielordner wählen",
                FileName = "Ordner auswählen"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TargetPath = Path.GetDirectoryName(saveFileDialog.FileName) ?? string.Empty;
                SendLogMessage(LogEntryType.TargetSelected, TargetPath);
            }
        }

        [RelayCommand]
        private async Task ProcessCsvAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(CsvPath) || string.IsNullOrEmpty(TargetPath))
                {
                    await ShowMessageAsync("Bitte wählen Sie sowohl eine Csv-Datei als auch einen Zielornder aus.");
                    return;
                }

                using (var reader = new StreamReader(CsvPath, true))
                {
                    var lines = await File.ReadAllLinesAsync(CsvPath, reader.CurrentEncoding);
                    await ProcessLinesAsync(lines);
                }
            }
            catch (Exception ex)
            {
                await ShowMessageAsync($"Fehler bei der Verarbeitung: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessLinesAsync(string[] lines)
        {
            if (lines.Length == 0)
            {
                await ShowMessageAsync("Die CSV-Datei ist leer.");
                return;
            }

            //Zurücksetzen der Statistik vor der Verarbeitung
            ResetStatistics();

            //Verarbeitung alle Zeilen außer Header-Zeile
            foreach (var line in lines.Skip(1))
            {
                await ProcessSingleLineAsync(line);
            }

            //Zeige Zusammenfassung am Ende
            await ShowSummaryAsync();
        }

        private void ResetStatistics()
        {
            CreatedFolders = 0;
            CreatedSubFolders = 0;
            ExistingFolders = 0;
            ExistingSubFolders = 0;
            CorrectedNames = 0;
        }

        private async Task ProcessSingleLineAsync(string line)
        {
            var values = line.Split(';').Select(x => x.Trim()).ToList();
            if (values.Count < 1) return;

            //Verarbeite Hauptordner
            var mainFolderResult = await ProcessMainFolderAsync(values[0]);
            if (!mainFolderResult.success) return;

            //Verarbeite Unterordner
            for (int i = 1; i < values.Count; i++)
            {
                await ProcessSubFolderAsync(values[i], mainFolderResult.path, mainFolderResult.name);
            }
        }

        private async Task ProcessSubFolderAsync(string originalName, string mainPath, string mainFolder)
        {
            if (string.IsNullOrWhiteSpace(originalName)) return;

            try
            {
                var correctedName = await NameCorrectionViewModel.CorrectetFolderNameAsync(originalName);
                if (correctedName != originalName)
                {
                    CorrectedNames++;
                    SendLogMessage(LogEntryType.NameCorrected, string.Empty, originalName, correctedName);
                }

                var fullPath = Path.Combine(mainPath, correctedName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    CreatedSubFolders++;
                    SendLogMessage(LogEntryType.SubFolderCreated, correctedName);
                }
                else
                {
                    ExistingSubFolders++;
                    SendLogMessage(LogEntryType.SubFolderCreated, correctedName);
                }
            }
            catch (OperationCanceledException)
            {
                SendLogMessage(LogEntryType.Error, "Vorgang abgeborchen");
            }
        }

        private async Task<(bool success, string path, string name)> ProcessMainFolderAsync(string originalName)
        {
            try
            {
                var correctedName = await NameCorrectionViewModel.CorrectetFolderNameAsync(originalName);
                if (correctedName != originalName)
                {
                    CorrectedNames++;
                    SendLogMessage(LogEntryType.NameCorrected, string.Empty, originalName, correctedName);
                }

                var fullPath = Path.Combine(TargetPath, correctedName);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    CreatedFolders++;
                    SendLogMessage(LogEntryType.MainFolderCreated, correctedName);
                }
                else
                {
                    ExistingFolders++;
                    SendLogMessage(LogEntryType.MainFolderExists, correctedName);
                }

                return (true, fullPath, correctedName);
            }
            catch (OperationCanceledException)
            {
                SendLogMessage(LogEntryType.Error, "Vorgang abgeborchen");
                return (false, string.Empty, string.Empty);
            }
        }

        private async Task ShowSummaryAsync()
        {
            await ShowMessageAsync(
                    $"📁 Verarbeitung abgeschlossen!\n\n" +
                    $"└─ Hauptordner\n" +
                    $"   ├─ Erstellt: {CreatedFolders}\n" +
                    $"   └─ Existierend: {ExistingFolders}\n\n" +
                    $"└─ Unterordner\n" +
                    $"   ├─ Erstellt: {CreatedSubFolders}\n" +
                    $"   └─ Existierend: {ExistingSubFolders}\n\n" +
                    $"└─ Korrekturen\n" +
                    $"   └─ {CorrectedNames}");
        }

        private async Task ShowMessageAsync(string m)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            MessageBox.Show(m));
        }

        private void SendLogMessage(LogEntryType type, string message, params string[] parameters)
        {
            _messenger.Send(new LogMessage(type, message, parameters));
        }

     

        // Hauptkonstruktor
        public MainViewModel(IMessenger messenger, LoggingService loggingService)
            : base(messenger)
        {
            _messenger = messenger;
            _loggingService = loggingService;

            // Log-Updates abonnieren
            _loggingService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LoggingService.CurrentLog))
                {
                    StatusMessage = _loggingService.CurrentLog;
                }
            };

            Initialize();
        }

        public override void Initialize()
        {
           // base.Initialize();
            // Sample Data Input //
        }
    }
}