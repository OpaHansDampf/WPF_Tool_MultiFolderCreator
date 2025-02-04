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
                LogStatus($"CSV-Datei ausgewählt: {CsvPath}");
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
                LogStatus($"Zielordner ausgewählt: {TargetPath}");
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
                    LogStatus($"Unterordner erstellt: {originalName} -> {correctedName}");
                }

                var fullPath = Path.Combine(mainPath, correctedName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    CreatedSubFolders++;
                    LogStatus($"Unterordner erstellt: {mainFolder}/{correctedName}");
                }
                else
                {
                    ExistingSubFolders++;
                    LogStatus($"Unterordner existiert bereits: {mainFolder}/{correctedName}");
                }
            }
            catch (OperationCanceledException)
            {
                LogStatus("Vorgang abgeborchen");
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
                    LogStatus($"Hauptordnername korrigiert: {originalName} -> {correctedName}");
                }

                var fullPath = Path.Combine(TargetPath, correctedName);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    CreatedFolders++;
                    LogStatus($"Hauptordner erstellt: {correctedName}");
                }
                else
                {
                    ExistingFolders++;
                    LogStatus($"Hauptordner existiert bereits: {correctedName}");
                }

                return (true, fullPath, correctedName);
            }
            catch (OperationCanceledException)
            {
                LogStatus("Vorgang abgebrochen.");
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

        private void LogStatus(string message)
        {
            string formattedMessage = FormatLogMessage(message);
            StatusMessage += formattedMessage;
        }

        private string FormatLogMessage(string message)
        {
            string timestamp = $"{DateTime.Now:HH:mm:ss}";
            string prefix;
            string icon;

            // Message-Typ bestimmen und entsprechend formatieren
            if (message.StartsWith("CSV-Datei ausgewählt:") || message.StartsWith("Zielordner ausgewählt:"))
            {
                // Pfad-Informationen ohne Baum
                return $"{timestamp}: {message}{Environment.NewLine}";
            }

            if (message.Contains("Hauptordnername korrigiert:"))
            {
                icon = "↺";
                prefix = "├─";
                message = message.Replace("Hauptordnername korrigiert:", "Umbenannt:");
                return $"{timestamp}: {prefix} {icon} {message}{Environment.NewLine}";
            }

            if (message.Contains("Hauptordner erstellt:"))
            {
                icon = "📁";
                prefix = "├─";
            }
            else if (message.Contains("Unterordner erstellt:"))
            {
                icon = "📂";
                prefix = "│  ├─";
                // Pfad kürzen - nur den letzten Teil anzeigen
                var parts = message.Split('/');
                if (parts.Length > 1)
                {
                    message = $"Unterordner: {parts.Last()}";
                }
            }
            else
            {
                icon = "•";
                prefix = "├─";
            }

            return $"{timestamp}: {prefix} {icon} {message}{Environment.NewLine}";
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