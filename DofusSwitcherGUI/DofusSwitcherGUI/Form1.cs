using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Input;
using System.Security.Permissions;
using HardwareHelperLib;
using System.Management;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;

namespace DofusSwitcherGUI {
    public partial class Form1 : Form {

        // Variables
        ProcessManager processManager;
        AccountInfos[] accountData;
        private MouseHookListener m_mouseListener;


        // Bibliothèques
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();


        bool pressed = false;
        bool modDeplacement = false;
        bool isRunning = true;
        bool clicked = false;
        bool automaticConnection = false;
        Thread TH;
        Thread TH2;
        Thread TH3;
        Thread TH4;
        Rectangle BoundRect;
        Rectangle OldRect = Rectangle.Empty;
        ManagementEventWatcher startWatch;
        ManagementEventWatcher stopWatch;
        bool test = false;
        int countAccountButtonCheckClick = 0;
        int checkAccount = 0;

        public Form1() {
            InitializeComponent();
            processManager = new ProcessManager(this);
            Reader read = new Reader();
            accountData = read.test();
            countAccountButtonCheckClick = processManager.checkDofusProcess(accountData);
            m_mouseListener = new MouseHookListener(new GlobalHooker());
            m_mouseListener.Enabled = true;
            m_mouseListener.MouseDownExt += CheckButtonMiddle;
            WaitForNewAndForExitedDofusProcess();
        }

        public ListBox getListbox() {
            return listBox1;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                Thread.Sleep(40);
                SetForegroundWindow(Process.GetProcessById((((ComboboxItem)listBox1.Items[listBox1.SelectedIndex]).Value)).MainWindowHandle);
                ShowWindow(Process.GetProcessById((((ComboboxItem)listBox1.Items[listBox1.SelectedIndex]).Value)).MainWindowHandle,5);
            }
            catch(Exception exce) {

            }
        }

        public void WaitForNewAndForExitedDofusProcess() {
            try {
                startWatch = new ManagementEventWatcher(
                  new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived
                                    += new EventArrivedEventHandler(startWatch_EventArrived);
                startWatch.Start();

                stopWatch = new ManagementEventWatcher(
                    new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived
                                    += new EventArrivedEventHandler(stopWatch_EventArrived);
                stopWatch.Start();
            }
            catch(Exception exception) {

            }
        }

        public void startWatch_EventArrived(object sender, EventArrivedEventArgs e) {
            if((String)e.NewEvent.Properties["ProcessName"].Value == "dofus.dll") {
                ComboboxItem item = new ComboboxItem();
                item.Value = Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString());
                if(automaticConnection) {
                    item.Text = accountData[checkAccount].classe;
                }
                else {
                    item.Text = "Dofus";
                }
                listBox1.Items.Add(item);
                listBox1.Update();
                countAccountButtonCheckClick++;
            }
        }

        public void stopWatch_EventArrived(object sender, EventArrivedEventArgs e) {
            if((String)e.NewEvent.Properties["ProcessName"].Value == "dofus.dll") {
                foreach(ComboboxItem x in listBox1.Items) {
                    if(x.Value == Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString())) {
                        listBox1.Items.Remove(x);
                        listBox1.Update();
                        countAccountButtonCheckClick--;
                    }
                }
            }
        }

        private void createNewThread(Thread th, ThreadStart start) {
            th = new Thread(start);
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = true;
            CheckForIllegalCrossThreadCalls = false;
            th.Start();
        }

        private void Form1_Load(object sender, EventArgs e) {
            createNewThread(TH, KeyboardControlUP);
            createNewThread(TH2, MouseThread);
            //createNewThread(TH3, CheckButtonMiddle);
            createNewThread(TH4, KeyboardTabDown);
        }

        private void Form1_Close(object sender, EventArgs e) {
            KillAllThread();
            startWatch.Stop();
            stopWatch.Stop();
            Thread.Sleep(50);
            Application.Exit();
        }


        public void KeyboardControlUP() {
            while(isRunning) {
                Thread.Sleep(40);
                switchEcranWithKeyUpWithControl();
            }
        }

        public void KeyboardTabDown() {
            while(isRunning) {
                Thread.Sleep(40);
                switchEcranWithKeyDownWithTab();
                Console.WriteLine("SALUT");
            }
        }

        public void MouseThread() {
            m_mouseListener.MouseDownExt += switchEcranWithMouse;

            while(isRunning) {
                Thread.Sleep(475);
                if(clicked) {
                    if(listBox1.SelectedIndex < listBox1.Items.Count - 1) {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        clicked = false;
                    }
                    else if(listBox1.SelectedIndex == listBox1.Items.Count - 1) {
                        listBox1.SelectedIndex = 0;
                        clicked = false;
                    }
                }
            }

        }



        public void CheckButtonMiddle(object sender, MouseEventExtArgs e) {
            Thread.Sleep(50);
            if(listBox1.Items.Count > 1) {
                if(e.Button == MouseButtons.Middle & !test) {
                    listBox1.SelectedIndex = 0;
                    label7.ForeColor = Color.Red;
                    label7.Text = "activé ";
                    modDeplacement = true;
                    test = true;
                }
                else if(e.Button == MouseButtons.Middle & test) {
                    label7.ForeColor = Color.Green;
                    label7.Text = "désactivé ";
                    modDeplacement = false;
                    test = false;
                }
            }
        }



        public void switchEcranWithKeyDownWithTab() {
            if(listBox1.Items.Count > 1) {
                if(Keyboard.IsKeyDown(Key.Tab) & !pressed) {
                    Thread.Sleep(50);
                    if(Keyboard.IsKeyUp(Key.Tab) & !pressed)
                        Thread.Sleep(50);
                    if(listBox1.SelectedIndex > 0) {
                        listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                    }
                    else if(listBox1.SelectedIndex == 0) {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }
                    pressed = true;
                }
                else if((Keyboard.IsKeyUp(Key.Tab)) & pressed) {
                    Thread.Sleep(50);
                    pressed = false;
                }
            }
        }

        public void switchEcranWithKeyUpWithControl() {
            if(checkBox1.Checked) {
                if(listBox1.Items.Count > 1) {
                    if(Keyboard.IsKeyDown(Key.LeftCtrl) & !pressed) {
                        Thread.Sleep(50);
                        if(Keyboard.IsKeyUp(Key.LeftCtrl) & !pressed)
                            Thread.Sleep(50);
                        if(listBox1.SelectedIndex < listBox1.Items.Count - 1) {
                            listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        }
                        else if(listBox1.SelectedIndex == listBox1.Items.Count - 1) {
                            listBox1.SelectedIndex = 0;
                        }
                        pressed = true;
                    }
                    else if((Keyboard.IsKeyUp(Key.LeftCtrl)) & pressed) {
                        Thread.Sleep(50);
                        pressed = false;
                    }
                }
            }
        }


        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void KillAllThread() {
            TH.Interrupt();
            TH.Abort();
            TH2.Interrupt();
            TH2.Abort();
            TH4.Interrupt();
            TH4.Abort();
        }

        public void switchEcranWithMouse(object sender, MouseEventExtArgs e) {
            int x;
            int y;
            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

            if(listBox1.Items.Count > 1) {
                if(e.Button == MouseButtons.Left & modDeplacement) {
                    x = p.X;
                    y = p.Y;
                    clicked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if(listBox1.Items.Count == 0) {
                automaticConnection = true;
                for(int i = 0; i < Reader.countLine; i++) {
                    connexionAutomatique(i);
                    checkAccount++;
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e) {
            System.Windows.Forms.MouseEventArgs me = (System.Windows.Forms.MouseEventArgs)e;
                if(listBox1.SelectedItem != null) {
                    Point p = System.Windows.Forms.Cursor.Position;
                    int index = this.listBox1.SelectedIndex;
                    Form2 textForm = new Form2(this, index);
                    textForm.Location = new Point(p.X, p.Y);
                    textForm.Show();
                    Console.WriteLine(((ComboboxItem)listBox1.SelectedItem).Text);
                }
            
        }

        private void button2_Click(object sender, EventArgs e) {
            string path = "J:\\Jeux video\\Dofus\\Dofus.exe";
            Process dofusProcess = Process.Start(path);
            Thread.Sleep(1500);
            foreach(int x in listBox1.SelectedIndices) {
                if(dofusProcess.Id == ((ComboboxItem)listBox1.Items[x]).Value) {
                    listBox1.SelectedIndex=x;
                }
            }
            Thread.Sleep(100);
            SendKeys.Send("%" + " " + "N");
        }

        private void connexionAutomatique(int _checkAccount) {
            string path = "J:\\Jeux video\\Dofus\\Dofus.exe";
            Process dofusProcess = Process.Start(path);
            Thread.Sleep(1500);
            foreach(int x in listBox1.SelectedIndices) {
                if(dofusProcess.Id == ((ComboboxItem)listBox1.Items[x]).Value) {
                    listBox1.SelectedIndex = x;
                }
            }
            Thread.Sleep(100);
            SendKeys.Send("%" + " " + "N");
            Thread.Sleep(3000);
                SendKeys.Send(accountData[_checkAccount].account);
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send(accountData[_checkAccount].pass);
                Thread.Sleep(300);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{ENTER}");
                Thread.Sleep(6000);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                if(accountData[_checkAccount].pseudoLong == "true") {
                    SendKeys.Send("{TAB}");
                    Thread.Sleep(100);
                }
                SendKeys.Send("{ENTER}");
                Thread.Sleep(150);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{TAB}");
                Thread.Sleep(100);
                SendKeys.Send("{ENTER}");
           
        }

        private void button3_Click(object sender, EventArgs e) {
            DialogResult dialogResult = MessageBox.Show("T'es sur ma poule ?", "Boite de validatin de quittatin de toutlescomte bim", MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.Yes) {
                foreach(ComboboxItem item in listBox1.Items) {
                    Process.GetProcessById(item.Value).CloseMainWindow();
                }
            }
            else if(dialogResult == DialogResult.No) {
            }
        }


        private void MouseListener_MouseDownExt(object sender, MouseEventExtArgs e) {
            // log the mouse click
            Console.WriteLine(string.Format("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp));

            // uncommenting the following line with suppress a middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e) {

        }

        private void button4_Click(object sender, EventArgs e) {
            DialogResult result = openFileDialog1.ShowDialog();
            if(result == DialogResult.OK) // Test result.
            {
            }
            Console.WriteLine(result); // <-- For debugging use.
        }
    }
}

