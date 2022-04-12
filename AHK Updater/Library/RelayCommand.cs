﻿using System;
using System.Windows.Input;

namespace AHKUpdater.Library
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand ( Action<T> execute )
           : this( execute, null )
        {
            _execute = execute;
        }

        public RelayCommand ( Action<T> execute, Predicate<T> canExecute )
        {
            _execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute ( object parameter )
        {
            return _canExecute == null || _canExecute( (T) parameter );
        }

        public void Execute ( object parameter )
        {
            _execute( (T) parameter );
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand ( Action<object> execute )
           : this( execute, null )
        {
            _execute = execute;
        }

        public RelayCommand ( Action<object> execute, Predicate<object> canExecute )
        {
            _execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
            _canExecute = canExecute;
        }

        // Ensures WPF commanding infrastructure asks all RelayCommand objects whether their
        // associated views should be enabled whenever a command is invoked 
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        private event EventHandler CanExecuteChangedInternal;

        public bool CanExecute ( object parameter )
        {
            return _canExecute == null || _canExecute( parameter );
        }

        public void Execute ( object parameter )
        {
            _execute( parameter );
        }

        public void RaiseCanExecuteChanged ()
        {
            CanExecuteChangedInternal.Raise( this );
        }
    }

    public static class EventRaiser
    {
        public static void Raise ( this EventHandler handler, object sender )
        {
            handler?.Invoke( sender, EventArgs.Empty );
        }

        public static void Raise<T> ( this EventHandler<EventArgs<T>> handler, object sender, T value )
        {
            handler?.Invoke( sender, new EventArgs<T>( value ) );
        }

        public static void Raise<T> ( this EventHandler<T> handler, object sender, T value ) where T : EventArgs
        {
            handler?.Invoke( sender, value );
        }

        public static void Raise<T> ( this EventHandler<EventArgs<T>> handler, object sender, EventArgs<T> value )
        {
            handler?.Invoke( sender, value );
        }
    }

    public class EventArgs<T> : EventArgs
    {
        public EventArgs ( T value )
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}
