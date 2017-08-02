using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GraphSharpDemo.ViewModels
{
    public class NodeViewModel
    {
        public string Name { get; set; }

        public int Defense { get; set; }

        public string DefenseInfo { get; set; }

        public string Type { get; set; }

        public ObservableCollection<int> AttackCodes { get; set; }

        public bool IsHackable { get { return AttackCodes.Count == 0; } }

        public bool IsDisabled { get; set; }

        public bool IsLocked { get; set; }

        public TimeSpan DisabledTimeLeft { get; set; }

        public TimeSpan LockedTimeLeft { get; set; }

        public bool IsExplored { get; set; }

        public string DefenseLog{ get; set; }

        public bool IsSelected { get; set; }
    }
}
