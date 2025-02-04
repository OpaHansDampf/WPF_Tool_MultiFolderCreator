using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Tool_MultiFolderCreator.Services.Logging
{
    public record LogMessage(LogEntryType Type, string Message, string[]? Parameters = null);

    public enum LogEntryType
    {
        CsvSelected,
        TargetSelected,
        MainFolderCreated,
        SubFolderCreated,
        NameCorrected,
        MainFolderExists,
        SubFolderExists,
        Error
    }
}
