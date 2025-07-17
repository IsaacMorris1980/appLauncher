using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input; // Required for ICommand

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels, providing common functionality like
    /// INotifyPropertyChanged implementation and custom Command types.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.
        /// This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.
        /// This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// A basic implementation of ICommand for synchronous operations.
        /// </summary>
        public class Command : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="Command"/> class.
            /// </summary>
            /// <param name="execute">The action to execute when the command is invoked.</param>
            /// <param name="canExecute">Optional function to determine if the command can execute.</param>
            public Command(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            /// <returns>True if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    _execute();
                }
            }

            /// <summary>
            /// Raises the <see cref="CanExecuteChanged"/> event.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// A basic implementation of ICommand for synchronous operations with a parameter.
        /// </summary>
        /// <typeparam name="T">The type of the command parameter.</typeparam>
        public class Command<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;

            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="Command{T}"/> class.
            /// </summary>
            /// <param name="execute">The action to execute when the command is invoked.</param>
            /// <param name="canExecute">Optional function to determine if the command can execute.</param>
            public Command(Action<T> execute, Func<T, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command. This object is cast to type T.</param>
            /// <returns>True if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute((T)parameter);
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="parameter">Data used by the command. This object is cast to type T.</param>
            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    _execute((T)parameter);
                }
            }

            /// <summary>
            /// Raises the <see cref="CanExecuteChanged"/> event.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// A basic implementation of ICommand for asynchronous operations.
        /// </summary>
        public class AsyncCommand : ICommand
        {
            private readonly Func<Task> _execute;
            private readonly Func<bool> _canExecute;
            private bool _isExecuting;

            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
            /// </summary>
            /// <param name="execute">The asynchronous function to execute when the command is invoked.</param>
            /// <param name="canExecute">Optional function to determine if the command can execute.</param>
            public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            /// <returns>True if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return !_isExecuting && (_canExecute == null || _canExecute());
            }

            /// <summary>
            /// Executes the command asynchronously.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            public async void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    try
                    {
                        _isExecuting = true;
                        RaiseCanExecuteChanged();
                        await _execute();
                    }
                    finally
                    {
                        _isExecuting = false;
                        RaiseCanExecuteChanged();
                    }
                }
            }

            /// <summary>
            /// Raises the <see cref="CanExecuteChanged"/> event.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// A basic implementation of ICommand for asynchronous operations with a parameter.
        /// </summary>
        /// <typeparam name="T">The type of the command parameter.</typeparam>
        public class AsyncCommand<T> : ICommand
        {
            private readonly Func<T, Task> _execute;
            private readonly Func<T, bool> _canExecute;
            private bool _isExecuting;

            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
            /// </summary>
            /// <param name="execute">The asynchronous function to execute when the command is invoked.</param>
            /// <param name="canExecute">Optional function to determine if the command can execute.</param>
            public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command. This object is cast to type T.</param>
            /// <returns>True if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return !_isExecuting && (_canExecute == null || _canExecute((T)parameter));
            }

            /// <summary>
            /// Executes the command asynchronously.
            /// </summary>
            /// <param name="parameter">Data used by the command. This object is cast to type T.</param>
            public async void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    try
                    {
                        _isExecuting = true;
                        RaiseCanExecuteChanged();
                        await _execute((T)parameter);
                    }
                    finally
                    {
                        _isExecuting = false;
                        RaiseCanExecuteChanged();
                    }
                }
            }

            /// <summary>
            /// Raises the <see cref="CanExecuteChanged"/> event.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
