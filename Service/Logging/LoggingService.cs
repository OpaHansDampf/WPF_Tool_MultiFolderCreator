using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPF_Tool_MultiFolderCreator.Services.Logging
{
    public partial class LoggingService : ObservableObject
    {
        private readonly StringBuilder _logBuilder = new();

        [ObservableProperty]
        private string currentLog = string.Empty;

        private readonly IMessenger _messenger;

        public LoggingService(IMessenger messenger)
        {
            _messenger = messenger;
            _messenger.Register<LogMessage>(this, (r, m) => HandleLogMessage(m));
        }

        private void HandleLogMessage(LogMessage message)
        {
            string formattedMessage = FormatLogMessage(message);
            _logBuilder.Append(formattedMessage);
            CurrentLog = _logBuilder.ToString();

            // Zum Ende scrollen
            if (App.Current.MainWindow != null)
            {
                var rtb = App.Current.MainWindow.FindName("tb_Status") as TextBox;
                rtb?.ScrollToEnd();
            }
        }

        private string FormatLogMessage(LogMessage entry)
        {
            string timestamp = $"{DateTime.Now:HH:mm:ss}";

            return entry.Type switch
            {
                LogEntryType.CsvSelected => $"{timestamp}: 📄 CSV-Datei ausgewählt: {entry.Message}{Environment.NewLine}",
                LogEntryType.TargetSelected => $"{timestamp}: 📂 Zielordner ausgewählt: {entry.Message}{Environment.NewLine}",
                LogEntryType.MainFolderCreated => $"{timestamp}: ├─ 📁 Hauptordner erstellt: {entry.Message}{Environment.NewLine}",
                LogEntryType.SubFolderCreated => $"{timestamp}: │  ├─ 📂 Unterordner: {entry.Message}{Environment.NewLine}",
                LogEntryType.NameCorrected => $"{timestamp}: ├─ ↺ Umbenannt: {entry.Parameters?[0]} -> {entry.Parameters?[1]}{Environment.NewLine}",
                LogEntryType.MainFolderExists => $"{timestamp}: ├─ 📁 Hauptordner existiert bereits: {entry.Message}{Environment.NewLine}",
                LogEntryType.SubFolderExists => $"{timestamp}: │  ├─ 📂 Unterordner existiert bereits: {entry.Message}{Environment.NewLine}",
                LogEntryType.Error => $"{timestamp}: ❌ Fehler: {entry.Message}{Environment.NewLine}",
                _ => $"{timestamp}: • {entry.Message}{Environment.NewLine}"
            };
        }

        public void Clear()
        {
            _logBuilder.Clear();
            CurrentLog = string.Empty;
        }
    }
}