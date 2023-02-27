using System;
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

    public IEnumerator Execute(string command, string output, Action<string> callback)
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
        Process bashProcess = Process.Start(startInfo);

        while (!bashProcess.HasExited)
        {
            yield return null;
        }
        
        StreamReader reader = new StreamReader(logPath, true);
        string log = reader.ReadToEnd();
        reader.Close();

        callback.Invoke(log);

        yield break;
    }

}
