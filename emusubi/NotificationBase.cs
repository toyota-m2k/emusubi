using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emusubi
{
    public class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Notify(string[] propertyNames)
        {
            if (null != PropertyChanged)
            {
                foreach (var name in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        protected bool UpdateAndNotify<T>(string propertyName, ref T target, T value)
        {
            if(!target.Equals(value))
            {
                target = value;
                Notify(propertyName);
                return true;
            }
            return false;
        }

        protected bool UpdateAndNotify<T>(string[] propertyNames, ref T target, T value)
        {
            if (!target.Equals(value))
            {
                target = value;
                Notify(propertyNames);
                return true;
            }
            return false;
        }
    }
}
