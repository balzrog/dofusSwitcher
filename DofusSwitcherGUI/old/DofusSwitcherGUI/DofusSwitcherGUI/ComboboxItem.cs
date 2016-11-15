using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusSwitcherGUI {
    public class ComboboxItem {
        public string Text { get; set; }
        public int Value { get; set; }
        public object o { get; set; }
        public string Pseudo { get; set; }
       
        public override string ToString() {
            return Text;
        }
    }
}