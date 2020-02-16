using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Client.Desktop.ViewModels.Common.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            this.RaisePropertyChanged<T>(propertyExpression);
            return true;
        }

        protected bool Set<T>(ref T field, T newValue, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            T oldValue = field;
            field = newValue;
            this.RaisePropertyChanged<T>(propertyName);
            return true;
        }

        public virtual void RaisePropertyChanged<T>(string propertyExpression)
        {
            if (this.PropertyChanged == null)
                return;
            string propertyName = propertyExpression;
            if (string.IsNullOrEmpty(propertyName))
                return;
            this.RaisePropertyChanged(propertyName);
        }


        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (this.PropertyChanged == null)
                return;
            string propertyName = GetPropertyName<T>(propertyExpression);
            if (string.IsNullOrEmpty(propertyName))
                return;
            this.RaisePropertyChanged(propertyName);
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
            MemberExpression body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            PropertyInfo member = body.Member as PropertyInfo;
            if (member == null)
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            return member.Name;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action execute)
          : this(execute, (Func<bool>)null)
        {
        }

        /// <summary>Initializes a new instance of the RelayCommand class.</summary>
        /// <param name="execute">The execution logic. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = new Action(execute);
            if (canExecute == null)
                return;
            this._canExecute = new Func<bool>(canExecute);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand.CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged == null)
                return;
            canExecuteChanged((object)this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
                return true;
            
            return this._canExecute.Invoke();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public virtual void Execute(object parameter)
        {
            if (!this.CanExecute(parameter) || this._execute == null)
                return;
            this._execute.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action<T> execute)
          : this(execute, (Func<T, bool>)null)
        {
        }

        /// <summary>Initializes a new instance of the RelayCommand class.</summary>
        /// <param name="execute">The execution logic. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param>
        /// <param name="canExecute">The execution status logic. IMPORTANT: Note that closures are not supported at the moment
        /// due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/). </param>
        /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = execute;
            if (canExecute == null)
                return;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand`1.CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged == null)
                return;
            canExecuteChanged((object)this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to a null reference</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
                return true;
            return this._canExecute((T) parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to a null reference</param>
        public virtual void Execute(object parameter)
        {
            object parameter1 = parameter;
            if (!this.CanExecute(parameter1) || this._execute == null)
                return;
            if (parameter1 == null)
            {
                if (typeof(T).GetTypeInfo().IsValueType)
                    this._execute(default(T));
                else
                    this._execute((T)parameter1);
            }
            else
                this._execute((T)parameter1);
        }
    }

}
