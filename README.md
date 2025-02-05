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

Die Hauptfunktion `ProcessSubFolderAsync` �bernimmt die Verarbeitung eines Unterordners. Hier ist der vollst�ndige Code:
### Erkl�rung des Codes

1. **�berpr�fung des Ordnernamens**:
Der Code pr�ft, ob der urspr�ngliche Ordnername leer oder nur aus Leerzeichen besteht. Falls ja, wird die Funktion beendet.

2. **Korrektur des Ordnernamens**:
Der Ordnername wird asynchron korrigiert. Wenn der korrigierte Name vom urspr�nglichen abweicht, wird ein Log-Eintrag erstellt und der Z�hler f�r korrigierte Namen erh�ht.

3. **Erstellung des Unterordners**:
Der vollst�ndige Pfad des Unterordners wird erstellt. Wenn der Ordner nicht existiert, wird er erstellt und ein Log-Eintrag hinzugef�gt. Andernfalls wird ein Log-Eintrag f�r bereits existierende Ordner erstellt.

4. **Fehlerbehandlung**:
Falls der Vorgang abgebrochen wird, wird ein Fehler-Log-Eintrag erstellt.

## Zusammenfassung

Dieses Tool automatisiert die Erstellung und Verwaltung von Unterordnern und dokumentiert den Prozess durch Log-Nachrichten. Es ist besonders n�tzlich f�r die Verwaltung gro�er Verzeichnisstrukturen.
# WPF_Tool_MultiFolderCreator

Dieses Tool dient zur Erstellung und Verwaltung von Unterordnern in einem Hauptverzeichnis. Es korrigiert Ordnernamen und erstellt neue Unterordner, falls diese noch nicht existieren. Zudem werden Log-Nachrichten generiert, um den Prozess zu dokumentieren.

## Funktionsweise

Die Hauptfunktion `ProcessSubFolderAsync` �bernimmt die Verarbeitung eines Unterordners. Hier ist der vollst�ndige Code:
### Erkl�rung des Codes

1. **�berpr�fung des Ordnernamens**:
   Der Code pr�ft, ob der urspr�ngliche Ordnername leer oder nur aus Leerzeichen besteht. Falls ja, wird die Funktion beendet.

2. **Korrektur des Ordnernamens**:
   Der Ordnername wird asynchron korrigiert. Wenn der korrigierte Name vom urspr�nglichen abweicht, wird ein Log-Eintrag erstellt und der Z�hler f�r korrigierte Namen erh�ht.

3. **Erstellung des Unterordners**:
   Der vollst�ndige Pfad des Unterordners wird erstellt. Wenn der Ordner nicht existiert, wird er erstellt und ein Log-Eintrag hinzugef�gt. Andernfalls wird ein Log-Eintrag f�r bereits existierende Ordner erstellt.

4. **Fehlerbehandlung**:
   Falls der Vorgang abgebrochen wird, wird ein Fehler-Log-Eintrag erstellt.

## Beispiel Code zum Erstellen von Logs
## Zusammenfassung

Dieses Tool automatisiert die Erstellung und Verwaltung von Unterordnern und dokumentiert den Prozess durch Log-Nachrichten. Es ist besonders n�tzlich f�r die Verwaltung gro�er Verzeichnisstrukturen.
