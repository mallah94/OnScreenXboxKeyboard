﻿using System.Windows.Input;

namespace OnScreenXboxKeyboard.Helpers
{ /// <summary>
  /// Simplistic delegate command for the demo.
  /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; } = null!;
        public Func<bool> CanExecuteFunc { get; set; } = null!;

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}