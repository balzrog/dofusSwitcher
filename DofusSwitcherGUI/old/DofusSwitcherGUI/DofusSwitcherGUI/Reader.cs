using DofusSwitcherGUI;
using System;

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
        // Read the file and display it line by line.
        System.IO.StreamReader file =
            new System.IO.StreamReader(@"J:\Cours\Workspace C#\DofusSwitcherGUI\DofusSwitcherGUI\BDD.txt");
        while((line = file.ReadLine()) != null) {
            data = line.Split(separators,3);
            accountInfosTab[counter] = new AccountInfos(data[0], data[1], data[2]);
            counter++;
        }
        countLine = counter;
        file.Close();
        // Suspend the screen.
        return accountInfosTab;
    }
}
