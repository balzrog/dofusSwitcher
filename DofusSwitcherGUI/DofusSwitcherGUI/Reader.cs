using DofusSwitcherGUI;
using System;
using System.Diagnostics;
using System.IO;

public class Reader
{
    public static int countLine;
	public Reader()
	{

	}

    public AccountInfos[] test() {
        int counter = 0;
        string line;
        char[] separators = { ';' };
        String[] data;
        AccountInfos[] accountInfosTab = new AccountInfos[8];
        string fileName = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\BDD\BDD.txt";
        System.IO.StreamReader file =
            new System.IO.StreamReader(@"J:\Cours\Workspace C#\DofusSwitcherGUI\DofusSwitcherGUI\BDD.txt");
        while((line = file.ReadLine()) != null) {
            data = line.Split(separators,4);
            accountInfosTab[counter] = new AccountInfos(data[0], data[1], data[2],data[3]);
            counter++;
        }
        countLine = counter;
        file.Close();
        // Suspend the screen.
        return accountInfosTab;
    }
}
