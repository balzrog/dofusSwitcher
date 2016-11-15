using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusSwitcherGUI {
    public class AccountInfos {
        public String account { get; set; }
        public String pass { get; set; }
        public String classe { get; set; }

        public AccountInfos(String _account, String _pass, String _classe) {
            account = _account;
            pass = _pass;
            classe = _classe;
        }

        public void sucemoi() { }
    }
}
