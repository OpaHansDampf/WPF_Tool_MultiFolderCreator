using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WPF_Tool_MultiFolderCreator.Models
{
    internal class FolderNameModel
    {
        internal bool HasInvalidChars(string folderName)
        {
            var invalidChars = Path.GetInvalidFileNameChars() //In Methode auslagern 1/2
                .Concat(Path.GetInvalidPathChars())
                .Concat(new[] { ':', '\\', '/' });

            return folderName.Any(c => invalidChars.Contains(c));
        }

        internal string AutoCorrectFolderName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars() //In Methode auslagern 2/2
                .Concat(Path.GetInvalidPathChars())
                .Concat(new[] { ':', '\\', '/' });

            var result = name;
            foreach (var c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            //Entfernt mehrfache Unterstiche
            result = Regex.Replace(result, @"_{2,}", "_");

            //Entfernt Unterstriche am Anfang und Ende
            result = result.Trim('_');

            return result;
        }
    }
}