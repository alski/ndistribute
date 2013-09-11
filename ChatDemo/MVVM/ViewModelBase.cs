using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatDemo.MVVM
{
    public class ViewModelBase :INotifyPropertyChanged
    {
        public void NotifyChanged( [CallerMemberName] string propertyName="")
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetAndNotifyChanged<T>(ref T _backingField, T newValue, [CallerMemberName] string propertyName="")
        {
            if ((_backingField == null && newValue != null)
                || _backingField.Equals(newValue) == false)
            {
                _backingField = newValue;
                NotifyChanged(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
