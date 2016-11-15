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

namespace DofusSwitcherGUI {
    public partial class Form1 : Form {

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        bool pressed = false;
        bool modDeplacement = false;
        bool isRunning = true;
        bool mousePressed = false;
        Thread TH;
        Thread TH2;
        Thread TH3;
        Thread TH4;
        Rectangle BoundRect;
        Rectangle OldRect = Rectangle.Empty;
        ManagementEventWatcher startWatch;
        ManagementEventWatcher stopWatch;
        bool test = false;
        AccountInfos[] accountData;
        int compteurAccount = 0;
        int countAccountButtonCheckClick = 0;

        public Form1() {
            InitializeComponent();
            Reader read = new Reader();
            accountData = read.test();
            checkDofusProcess();
            WaitForNewAndForExitedDofusProcess();
        }

        private void checkDofusProcess() {
            
            Process[] dofus = Process.GetProcessesByName("dofus.dll");
            foreach(Process x in dofus) {
                ComboboxItem item = new ComboboxItem();
                item.Value = x.Id;
                item.Text = "Classe : " + accountData[compteurAccount].classe;
                listBox1.Items.Add(item);
                compteurAccount++;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                SetForegroundWindow(Process.GetProcessById((((ComboboxItem)listBox1.Items[listBox1.SelectedIndex]).Value)).MainWindowHandle);
                ShowWindow(Process.GetProcessById((((ComboboxItem)listBox1.Items[listBox1.SelectedIndex]).Value)).MainWindowHandle, 5);
            }catch(Exception exce) {

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
            }catch(Exception exception) {

            }
        }

        public void startWatch_EventArrived(object sender, EventArrivedEventArgs e) {
            if((String)e.NewEvent.Properties["ProcessName"].Value == "dofus.dll") {
                ComboboxItem item = new ComboboxItem();
                Console.WriteLine(Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString()));
                item.Value = Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString());
                item.Text = "Dofus id :" + Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString());
                listBox1.Items.Add(item);
                listBox1.Update();
                
            }
        }

        public void stopWatch_EventArrived(object sender, EventArrivedEventArgs e) {
            if((String)e.NewEvent.Properties["ProcessName"].Value == "dofus.dll") {
                foreach(ComboboxItem x in listBox1.Items){
                    if(x.Value == Int16.Parse(e.NewEvent.Properties["ProcessId"].Value.ToString())) {
                        listBox1.Items.Remove(x);
                        listBox1.Update();
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
            createNewThread(TH2, Mouseee);
            createNewThread(TH3, CheckButtonMiddle);
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
            }
        }

        public void Mouseee() {
            while(isRunning) {
                Thread.Sleep(40);
                switchEcranWithMouse();
            }
        }


        public void CheckButtonMiddle() {
            while(isRunning) {
                Thread.Sleep(50);
                if((Control.MouseButtons & MouseButtons.Middle) == MouseButtons.Middle & !test) {
                    Thread.Sleep(50);
                    if((Control.MouseButtons & MouseButtons.Middle) != MouseButtons.Middle & !test) {
                        label5.Text = "Mode déplacement : activé ";
                        modDeplacement = true;
                        test = true;
                    }
                }
                if((Control.MouseButtons & MouseButtons.Middle) == MouseButtons.Middle & test) {
                    Thread.Sleep(50);
                    if((Control.MouseButtons & MouseButtons.Middle) != MouseButtons.Middle & test) {
                        label5.Text = "Mode déplacement : désactivé ";
                        modDeplacement = false;
                        test = false;
                    }
                }
            }
        }


        public void switchEcranWithKeyDownWithTab() {
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

        public void switchEcranWithKeyUpWithControl() {
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


        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void KillAllThread() {
            TH.Interrupt();
            TH.Abort();
            TH2.Interrupt();
            TH2.Abort();
            TH3.Interrupt();
            TH3.Abort();
            // TH4.Interrupt();
            // TH4.Abort();
        }

        public void switchEcranWithMouse() {
           
            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
            try {
                label2.Text = "X " + p.X;
                label3.Text = "Y " + p.Y;
            }
            catch(Exception e) {

            }

            if(((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) & !mousePressed & modDeplacement) {
                Thread.Sleep(50);
                if(!((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) & !mousePressed) {
                    Thread.Sleep(50);
                    label4.Text = "CLIC 1";
                    if(listBox1.SelectedIndex < listBox1.Items.Count - 1) {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        label4.Text = "CLIC 2";
                    }
                    else if(listBox1.SelectedIndex == listBox1.Items.Count - 1) {
                        listBox1.SelectedIndex = 0;
                        label4.Text = "CLIC 3";
                    }
                    mousePressed = true;
                }
            }
            else if(!((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) & mousePressed) {
                Thread.Sleep(50);
                label4.Text = "PAS CLIC";
                mousePressed = false;
                //}
            }
            }

        private void button1_Click(object sender, EventArgs e) {
            if(countAccountButtonCheckClick < Reader.countLine) {

                Process.Start("J:\\Jeux video\\Dofus\\Dofus.exe","--maxi");
                countAccountButtonCheckClick++;
            }
        }
    }
}

