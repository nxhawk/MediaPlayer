using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaPlayer.Keys
{
    public class GlobalHotkey
    {
        public ModifierKeys modifierKeys { get; set; }

        public Key Key { get; set; }

        public Action Callback { get; set; }

        public bool canExecute { get; set; }

        public GlobalHotkey(ModifierKeys modifierKeys, Key key, Action callback, bool canExecute = true)
        {
            this.modifierKeys = modifierKeys;
            Key = key;
            Callback = callback;
            this.canExecute = canExecute;
        }
    }
}
