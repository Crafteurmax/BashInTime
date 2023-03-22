using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CommandExecuter : MonoBehaviour
{
    [SerializeField] private string fileRoot = "BashWork/root";
    [SerializeField] private string root = "BashWork";

    [SerializeField] private string executeFile = "execute.sh";
    [SerializeField] private string consoleLogsFile = "log.txt";
    [SerializeField] private string errorLogsFile = "log_error.txt";


    [SerializeField] private string timedOutMessage = "The Terminal has crashed. Restarting... \n";
    [SerializeField] private float maxWaitingDuration = 5.0f;

    private Process previousProcess = null;
    private float waitingTime;

    //Execution d'une commande
    public IEnumerator Execute(string command, string currentDirectory, Action<string, string, string> callback, string userCommand)
    {
        //Les chemins des differents fichiers utilises
        string scriptPath = root + "/" + executeFile;
        string logPath = root + "/" + consoleLogsFile;
        string errorPath = root + "/" + errorLogsFile;

        //Commande finale
        string finalCommand =
            "echo \"\" > BashWork/empty.txt\n" +
            "exec 0<BashWork/empty.txt\n" +
            "exec 1>" + logPath + "\n" +
            "exec 2>" + errorPath + "\n" +
            "cd " + fileRoot + currentDirectory + "\n" +
            command;


        //On attend que le processus precedent s'est termine par securité
        if(previousProcess == null)
        {
            yield return null;
        }

        //On ecrit le script
        WriteFile(scriptPath, finalCommand);

        //On lance le processus
        Process bashProcess = StartBashScript(scriptPath);
        previousProcess = bashProcess;

        //Par securite, on attend que le processus soit termine ou que le temps maximum soit depasse
        waitingTime = Time.time;
        while (!bashProcess.HasExited && Time.time < waitingTime + maxWaitingDuration)
        {
            yield return null;
        }

        //Si le temps a ete depasse, on tue le processus, et on met une message d'erreur
        if (Time.time >= waitingTime + maxWaitingDuration)
        {
            bashProcess.Kill();
            callback.Invoke("", timedOutMessage, command);
            yield break;
        }



        //On recupere les logs
        string log = ReadFile(logPath);
        string errorLog = ReadFile(errorPath);

        //Processus precedent termine
        previousProcess = null;

        
        //On appelle le callback
        callback.Invoke(log, errorLog, userCommand);

        yield break;
    }

    //Lire un fichier
    private string ReadFile(string path)
    {
        StreamReader reader = new StreamReader(path, true);
        string data = reader.ReadToEnd();
        reader.Close();

        return data;
    }

    //Ecrire un fichier
    private void WriteFile(string path, string data)
    {
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(data);
        writer.Close();
    }

    //Lancer un script bash en arriere-plan
    private Process StartBashScript(string scriptPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "C:\\Program Files\\Git\\git-bash.exe";
        startInfo.Arguments = "--hide  \"" + scriptPath + "\"";

        return Process.Start(startInfo);
    }

}
