using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwitcherGUI {
    class ProcessManager {
        int compteurAccount = 0;
        Form1 instance;

        public ProcessManager(Form1 _instance) {
            instance = _instance;
        }

        public int checkDofusProcess(AccountInfos[] _accountData) {
            Process[] dofus = Process.GetProcessesByName("dofus.dll");
            foreach(Process x in dofus) {
                ComboboxItem item = new ComboboxItem();
                item.Value = x.Id;
                if(_accountData[compteurAccount] == null) {
                    item.Text = "Dofus";
                }
                else {
                    item.Text = "Classe : " + _accountData[compteurAccount].classe;
                }
                instance.getListbox().Items.Add(item);
                compteurAccount++;
            }
            Console.WriteLine(compteurAccount);
            return compteurAccount;
        }
    }
}
