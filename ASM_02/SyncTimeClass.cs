using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_02
{
    public class SyncTimeClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string time;

        public string Time
        {
            get { return time; }
            set { time = value; OnPropertyChanged("Time"); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
