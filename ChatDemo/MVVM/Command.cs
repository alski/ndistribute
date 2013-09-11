using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ChatDemo.MVVM
{
    public class Command : ICommand 
    {
        private Func<object, bool> _canExecuteFunc;

        public Func<object, bool> CanExecuteFunc
        {
            get { return _canExecuteFunc; }
            set
            {
                _canExecuteFunc = value;
                OnCanExecuteFuncChanged();
            }
        }

        private void OnCanExecuteFuncChanged()
        {
            var temp = CanExecuteChanged;
            if(temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public Action<object> ExecuteAction { get; set; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }
    }
}
