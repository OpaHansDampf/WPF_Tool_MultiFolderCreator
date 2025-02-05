# WPF_Tool_MultiFolderCreator

Dieses Tool dient zur Erstellung und Verwaltung von Unterordnern in einem Hauptverzeichnis. Es korrigiert Ordnernamen und erstellt neue Unterordner, falls diese noch nicht existieren. Zudem werden Log-Nachrichten generiert, um den Prozess zu dokumentieren.

## Beispiel Code zum Erstellen von Logs:
# Short:
```
SendLogMessage(LogEntryType.NameCorrected, string.Empty, originalName, correctedName);
```
# Long:
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
## Funktionsweise

Die Hauptfunktion `ProcessSubFolderAsync` übernimmt die Verarbeitung eines Unterordners. Hier ist der vollständige Code:
### Erklärung des Codes

1. **Überprüfung des Ordnernamens**:
Der Code prüft, ob der ursprüngliche Ordnername leer oder nur aus Leerzeichen besteht. Falls ja, wird die Funktion beendet.

2. **Korrektur des Ordnernamens**:
Der Ordnername wird asynchron korrigiert. Wenn der korrigierte Name vom ursprünglichen abweicht, wird ein Log-Eintrag erstellt und der Zähler für korrigierte Namen erhöht.

3. **Erstellung des Unterordners**:
Der vollständige Pfad des Unterordners wird erstellt. Wenn der Ordner nicht existiert, wird er erstellt und ein Log-Eintrag hinzugefügt. Andernfalls wird ein Log-Eintrag für bereits existierende Ordner erstellt.

4. **Fehlerbehandlung**:
Falls der Vorgang abgebrochen wird, wird ein Fehler-Log-Eintrag erstellt.

## Zusammenfassung

Dieses Tool automatisiert die Erstellung und Verwaltung von Unterordnern und dokumentiert den Prozess durch Log-Nachrichten. Es ist besonders nützlich für die Verwaltung großer Verzeichnisstrukturen.
# WPF_Tool_MultiFolderCreator

Dieses Tool dient zur Erstellung und Verwaltung von Unterordnern in einem Hauptverzeichnis. Es korrigiert Ordnernamen und erstellt neue Unterordner, falls diese noch nicht existieren. Zudem werden Log-Nachrichten generiert, um den Prozess zu dokumentieren.

## Funktionsweise

Die Hauptfunktion `ProcessSubFolderAsync` übernimmt die Verarbeitung eines Unterordners. Hier ist der vollständige Code:
### Erklärung des Codes

1. **Überprüfung des Ordnernamens**:
   Der Code prüft, ob der ursprüngliche Ordnername leer oder nur aus Leerzeichen besteht. Falls ja, wird die Funktion beendet.

2. **Korrektur des Ordnernamens**:
   Der Ordnername wird asynchron korrigiert. Wenn der korrigierte Name vom ursprünglichen abweicht, wird ein Log-Eintrag erstellt und der Zähler für korrigierte Namen erhöht.

3. **Erstellung des Unterordners**:
   Der vollständige Pfad des Unterordners wird erstellt. Wenn der Ordner nicht existiert, wird er erstellt und ein Log-Eintrag hinzugefügt. Andernfalls wird ein Log-Eintrag für bereits existierende Ordner erstellt.

4. **Fehlerbehandlung**:
   Falls der Vorgang abgebrochen wird, wird ein Fehler-Log-Eintrag erstellt.

## Beispiel Code zum Erstellen von Logs
## Zusammenfassung

Dieses Tool automatisiert die Erstellung und Verwaltung von Unterordnern und dokumentiert den Prozess durch Log-Nachrichten. Es ist besonders nützlich für die Verwaltung großer Verzeichnisstrukturen.
