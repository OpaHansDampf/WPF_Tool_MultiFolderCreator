using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WPF_Tool_MultiFolderCreator;
using WPF_Tool_MultiFolderCreator.Views;

namespace WPF_Tool_MultiFolderCreator.Views
{
    public partial class MainWindow : Window
    {
        private string? _csvPath;
        private string? _targetPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool HasInvalidChars(string folderName)
        {
            // Erweiterte Liste von ungültigen Zeichen
            var invalidChars = Path.GetInvalidFileNameChars()
                .Concat(Path.GetInvalidPathChars())
                .Concat(new[] { ':', '\\', '/' });  // Zusätzliche problematische Zeichen

            return folderName.Any(c => invalidChars.Contains(c));
        }

        private string AutoCorrectFolderName(string name)
        {
            // Ersetzt problematische Zeichen durch Unterstriche
            var invalidChars = Path.GetInvalidFileNameChars()
                .Concat(Path.GetInvalidPathChars())
                .Concat(new[] { ':', '\\', '/' });

            var result = name;
            foreach (var c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            // Entfernt mehrfache Unterstriche
            result = Regex.Replace(result, @"_{2,}", "_");

            // Entfernt Unterstriche am Anfang und Ende
            result = result.Trim('_');

            return result;
        }

        private string CorrectFolderName(string originalName)
        {
            if (HasInvalidChars(originalName))
            {
                // Automatische Korrekturvorschlag generieren
                var suggestion = AutoCorrectFolderName(originalName);

                var dialog = new NameCorrectionDialog(originalName)
                {
                    Owner = this,
                    SuggestedName = suggestion  // Füge diese Property zum NameCorrectionDialog hinzu
                };

                if (dialog.ShowDialog() == true)
                {
                    var correctedName = dialog.CorrectedName;

                    // Prüfe ob der korrigierte Name noch ungültige Zeichen enthält
                    if (HasInvalidChars(correctedName))
                    {
                        MessageBox.Show("Der eingegebene Name enthält immer noch ungültige Zeichen. " +
                                      "Er wird automatisch korrigiert.", "Hinweis",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        return AutoCorrectFolderName(correctedName);
                    }
                    return correctedName;
                }

                throw new OperationCanceledException("Ordnererstellung abgebrochen durch Benutzer.");
            }
            return originalName;
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Zielordner auswählen",
                FileName = "Ordner auswählen"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _targetPath = Path.GetDirectoryName(saveFileDialog.FileName);
                FolderPathText.Text = _targetPath;
                LogStatus($"Zielordner ausgewählt: {_targetPath}");
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_csvPath) || string.IsNullOrEmpty(_targetPath))
            {
                MessageBox.Show("Bitte wählen Sie zuerst eine CSV-Datei und einen Zielordner aus.");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_csvPath, Encoding.UTF8);
                if (lines.Length == 0)
                {
                    MessageBox.Show("Die CSV-Datei ist leer.");
                    return;
                }

                int createdFolders = 0;
                int createdSubFolders = 0;
                int existingFolders = 0;
                int existingSubFolders = 0;
                int correctedNames = 0;

                // Erste Zeile überspringen (Header)
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(';').Select(x => x.Trim()).ToList();
                    if (values.Count >= 1)  // Mindestens ein Hauptordner
                    {
                        var originalMainFolder = values[0];
                        string mainFolder;

                        try
                        {
                            mainFolder = CorrectFolderName(originalMainFolder);
                            if (mainFolder != originalMainFolder)
                            {
                                correctedNames++;
                                LogStatus($"Hauptordnername korrigiert: {originalMainFolder} -> {mainFolder}");
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            LogStatus("Vorgang abgebrochen.");
                            return;
                        }

                        var mainPath = Path.Combine(_targetPath, mainFolder);

                        if (!Directory.Exists(mainPath))
                        {
                            Directory.CreateDirectory(mainPath);
                            createdFolders++;
                            LogStatus($"Hauptordner erstellt: {mainFolder}");
                        }
                        else
                        {
                            existingFolders++;
                            LogStatus($"Hauptordner existiert bereits: {mainFolder}");
                        }

                        // Verarbeite alle Unterordner (alle Spalten nach der ersten)
                        for (int i = 1; i < values.Count; i++)
                        {
                            var originalSubFolder = values[i];
                            if (!string.IsNullOrWhiteSpace(originalSubFolder))
                            {
                                string subFolder;
                                try
                                {
                                    subFolder = CorrectFolderName(originalSubFolder);
                                    if (subFolder != originalSubFolder)
                                    {
                                        correctedNames++;
                                        LogStatus($"Unterordnername korrigiert: {originalSubFolder} -> {subFolder}");
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    LogStatus("Vorgang abgebrochen.");
                                    return;
                                }

                                var subPath = Path.Combine(mainPath, subFolder);
                                if (!Directory.Exists(subPath))
                                {
                                    Directory.CreateDirectory(subPath);
                                    createdSubFolders++;
                                    LogStatus($"Unterordner erstellt: {mainFolder}/{subFolder}");
                                }
                                else
                                {
                                    existingSubFolders++;
                                    LogStatus($"Unterordner existiert bereits: {mainFolder}/{subFolder}");
                                }
                            }
                        }
                    }
                }

                var message = $"Ordner-Erstellung abgeschlossen!\n\n" +
                            $"Neue Hauptordner: {createdFolders}\n" +
                            $"Neue Unterordner: {createdSubFolders}\n" +
                            $"Bereits vorhandene Hauptordner: {existingFolders}\n" +
                            $"Bereits vorhandene Unterordner: {existingSubFolders}\n" +
                            $"Korrigierte Ordnernamen: {correctedNames}";

                MessageBox.Show(message, "Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Erstellen der Ordner: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogStatus(string message)
        {
            StatusTextBox.AppendText($"{message}{Environment.NewLine}");
            StatusTextBox.ScrollToEnd();
        }
    }
}


