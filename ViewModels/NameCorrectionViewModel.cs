using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Tool_MultiFolderCreator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;

namespace WPF_Tool_MultiFolderCreator.ViewModels
{
    public partial class NameCorrectionViewModel : ViewModelBase
    {
        private readonly FolderNameModel _folderNameModel;

        [ObservableProperty]
        private Action<bool?>? closeDialogAction;

        [ObservableProperty]
        private string originalName = string.Empty;

        [ObservableProperty]
        private string correctedName = string.Empty;

        [ObservableProperty]
        private bool canAccept = true;

        [ObservableProperty]
        private bool isProcessing;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
        private string userInput = string.Empty;

        public NameCorrectionViewModel()
        {
            _folderNameModel = new FolderNameModel();
        }

        [RelayCommand(CanExecute = nameof(CanAcceptName))]
        private async Task AcceptAsync()
        {
            try
            {
                IsProcessing = true;

                if (string.IsNullOrEmpty(UserInput))
                {
                    throw new InvalidOperationException("Name darf nicht leer sein");
                }

                await Task.Delay(100); //Simulierter Task

                CorrectedName = UserInput;

                if (_folderNameModel.HasInvalidChars(CorrectedName))
                {
                    // Falls die Benutzereingabe immer noch ungültige Zeichen enthält,
                    // automatisch korrigieren
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        CorrectedName = _folderNameModel.AutoCorrectFolderName(CorrectedName);
                    });
                }
                // Sichere Ausführung der CloseAction
                await Application.Current.Dispatcher.InvokeAsync(() => // TODO: Methode machen 1/2
                {
                    CloseDialogAction?.Invoke(true);
                });
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() => // TODO: Methode machen 2/2
            {
                CloseDialogAction?.Invoke(true);
            });
        }

        private bool CanAcceptName()
        {
            return !string.IsNullOrWhiteSpace(UserInput) &&
                   !IsProcessing &&
                   CanAccept;
        }

        partial void OnCloseDialogActionChanged(Action<bool?>? value)
        {
            if (value != null)
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    throw new InvalidOperationException(
                        "CloseDialogAction muss auf dem UI-Thread gesetzt werden");
                }
            }
        }

        public static async Task<string> CorrectetFolderNameAsync(string originalName)
        {
            var model = new FolderNameModel();

            if (!model.HasInvalidChars(originalName))
            {
                return originalName;
            }

            var vm = new NameCorrectionViewModel();
            vm.Initialize(originalName);

            var dialog = new Views.NameCorrectionDialog(vm);

            // Setze die CloseAction auf dem UI-Thread
            vm.CloseDialogAction = result => dialog.DialogResult = result; //WayToGo um eine Property als Action nutzen zu können

            if (await Task.Run(() => dialog.ShowDialog()) == true)
            {
                return vm.CorrectedName;
            }

            throw new OperationCanceledException("Benutzer hat den Vorgang abgebrochen");
        }

        public void Initialize(string name)
        {
            OriginalName = name;
            CorrectedName = _folderNameModel.AutoCorrectFolderName(name);
            UserInput = CorrectedName;
        }
    }
}