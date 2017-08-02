using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GraphSharpDemo.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isDisabled;
        public string Name { get; set; }

        public string Effect { get; set; }

        public long Defense { get; set; }

        public string DefenseInfo { get; set; }

        public string Type { get; set; }

        public ObservableCollection<int> AttackCodes { get; set; }

        public bool IsHackable { get { return AttackCodes.Count == 0; } }

        public bool IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                _isDisabled = value; 
                NotifyPropertyChanged("IsDisabled");
            }
        }

        public bool IsLocked { get; set; }

        public TimeSpan DisabledTimeLeft { get; set; }

        public TimeSpan LockedTimeLeft { get; set; }

        public bool IsExplored { get; set; }

        public string DefenseLog{ get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value; 
                NotifyPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
