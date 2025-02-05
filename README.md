# WPF_Tool_MultiFolderCreator

Beispiel Code zum erstellen von Logs:
```
SendLogMessage(LogEntryType.NameCorrected, string.Empty, originalName, correctedName);
```

```c#
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
            SendLogMessage(LogEntryType.SubFolderCreated, $"{mainFolder}/{correctedName}");
        }
        else
        {
            ExistingSubFolders++;
            SendLogMessage(LogEntryType.SubFolderExists, $"{mainFolder}/{correctedName}");
        }
    }
    catch (OperationCanceledException)
    {
        SendLogMessage(LogEntryType.Error, "Vorgang abgebrochen");
    }
}
```