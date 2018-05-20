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
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
        
    }
}
