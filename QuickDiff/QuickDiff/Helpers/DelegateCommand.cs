using System;
using System.Windows.Input;

namespace Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action Command;
        private readonly Func<bool> CanExecuteCommand;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action command, Func<bool> canExecuteCommand = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            CanExecuteCommand = canExecuteCommand;
            Command = command;
        }

        public void Execute(object parameter)
        {
            Command();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand == null || CanExecuteCommand();
        }

    }
}