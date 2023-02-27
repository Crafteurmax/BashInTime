using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CommandExecuter : MonoBehaviour
{
    public string root = "BashWork";

    public string executeFile = "execute.sh";
    public string consoleLogsFile = "log.txt";

    public string Execute(string command, string output)
    {

        if(output == null)
        {
            output = consoleLogsFile;
        }

        string logPath = root + "/" + consoleLogsFile;
        string finalCommand = command + ">" + logPath;
        

        string path = root+"/"+executeFile;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(finalCommand);
        writer.Close();


        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "C:\\Program Files\\Git\\git-bash.exe";
        startInfo.Arguments = "--hide  \""+path+"\"";
        UnityEngine.Debug.Log(startInfo.Arguments);
        Process bashProcess = Process.Start(startInfo);

        bashProcess.WaitForExit();
        
        StreamReader reader = new StreamReader(logPath, true);
        string log = reader.ReadToEnd();
        reader.Close();

        return log;
    }

}
