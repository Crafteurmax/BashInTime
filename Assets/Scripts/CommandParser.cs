using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CommandParser : MonoBehaviour
{
    //Racine reelle des commandes
    private string root = "BashWork/root";

    //CD virtuel
    private string currentDirectory = "/";

    private string authorizedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 \n\r\t/\\.-_";

    //Type de commande(voir ci-dessous)
    public enum CommandType : int
    {
        Error = -1,
        Direct = 0,
        File = 1,
        Interpreted = 2
    }

    
    public string[] fileCommands = {"ls", "rm", "grep", "mkdir", "cat", "touch", "cp", "find", "mv", "head", "tail"}; //Contient des arguments principaux qui sont des fichiers
    public string[] directCommands = { "echo"}; //Contient des arguments principaux qui sont des donnees directes
    public string[] interpretedCommands = { "cd", "pwd" }; //Commandes interpretees directement



    //Lien avec l'executeur de commandes
    private CommandExecuter executer;
    private void Awake()
    {
        executer = GetComponent<CommandExecuter>();
    }

    
    //Fonction Callback qui est appelee et recoit le message a afficher sur la console
    public void ShowReturnValue(string textLog, string errorLog, string command)
    {
        Debug.Log(textLog); //A changer
        if(errorLog.Trim() != "") Debug.LogError(errorLog);
    }

    //Fonction Principale d'execution d'une commande
    void ExecuteCommand(string line)
    {
        line = line.Trim();

        //On supprime les caracteres illegaux
        foreach(char c in line)
        {
            if (!authorizedCharacters.Contains(c))
            {
                ShowReturnValue("", "Illegal Character : '" + c + "'", line);
                return;
            }
        }


        //On recupere la commande et on appelle des fonctions differentes en fonction de celle-ci
        string[] words = line.Split(" ");

        if (words.Length <= 0) ShowReturnValue("", "Please Input something", line);

        string command = words[0].Trim();
        CommandType type = GetCommandType(command);

        switch (type)
        {
            case CommandType.Error:
                ShowReturnValue("", command + " of type " + type + " executed!", line);
                break;
            case CommandType.Direct:
                RawExecute(line, ShowReturnValue, line);
                break;
            case CommandType.File:
                SafeExecute(words, ShowReturnValue, line);
                break;
            case CommandType.Interpreted:
                Interprete(command, words, line);
                break;
        }

        return;
    }

    //Fonction qui recupere le type de commande
    public CommandType GetCommandType(string command)
    {
        foreach(string com in directCommands)
        {
            if (com.Equals(command)) return CommandType.Direct;
        }

        foreach (string com in fileCommands)
        {
            if (com.Equals(command)) return CommandType.File;
        }

        foreach (string com in interpretedCommands)
        {
            if (com.Equals(command)) return CommandType.Interpreted;
        }

        return CommandType.Error;
    }

    //Fonction qui recupere un chemin absolu virtuel (et relatif reel) a partir d'un chemin relatif virtuel (supprime les ..)
    private string GetAbsoluteVirtualPath(string path)
    {
        Stack<string> pathStack = new Stack<string>();
        string[] elements = path.Replace("\\","/").Split("/");
        
        //On utilise une pile pour resoudre les ..

        foreach(string element in elements)
        {
            switch (element)
            {
                case "":
                    break;
                case "..":
                    if(pathStack.Count > 0) pathStack.Pop();
                    break;
                case ".":
                    break;
                default:
                    pathStack.Push(element);
                    break;
            }
            
        }

        //On recontruit le chemin a partir de la pile

        string new_path = "";

        while(pathStack.Count > 0)
        {
            new_path = "/" + pathStack.Pop() + new_path;
        }

        new_path = "." + new_path;

        return new_path;
    }

    //Permet de savoir si une fonction est une option du type -blabla ou un argument principal
    private bool IsOption(string word)
    {
        if (word.Length == 0) return true;

        if (word[0] == '-')
        {
            for (int j = 0; j < word.Length; j++)
            {
                if (word[j] != '-')
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SafeExecute(string[] arguments, Action<string, string, string> callback, string userCommand)
    {

        //Detection d'arguments, safe car chemin relatifs transformés en chemins absolus virtuels
        for(int i = 1; i < arguments.Length; i++)
        {
            arguments[i] = IsOption(arguments[i]) ? arguments[i] : GetAbsoluteVirtualPath(arguments[i]);
        }

        string newCommand = "";

        foreach(string argument in arguments){
            newCommand += " " + argument;
        }

        RawExecute(newCommand, callback, userCommand);
    }

    //Execute directement la commande line
    private void RawExecute(string line, Action<string, string, string> callback, string userCommand)
    {
        StartCoroutine(executer.Execute(line, currentDirectory, callback, userCommand));

        return;
    }

    //Callback de la console pour CD, permet de savoir si le dossier existe
    public void CdCallback(string log, string errorLog, string command)
    {
        if (errorLog.Trim() != "")
        {
            ShowReturnValue(log, errorLog, command);
            return;
        }

        string[] arguments = command.Trim().Split(" ");

        //On recupere l'argument correspondant au fichier, et on le met sous la forme d'un chemin virtuel absolu.
        int fileIndex = 1;
        while (fileIndex < arguments.Length && IsOption(arguments[fileIndex])) fileIndex++;
        currentDirectory = ("/" + GetAbsoluteVirtualPath(currentDirectory + "/" + arguments[fileIndex])).Replace("/.","/").Replace("//","/");

        ShowReturnValue(log, errorLog, command);
    }

    //Switch d'interpretation des commandes
    private void Interprete(string command, string[] arguments, string userLine)
    {
        switch (command)
        {
            case "pwd":
                ShowReturnValue(currentDirectory, "", command);
                break;
            case "cd":
                SafeExecute(arguments, CdCallback, userLine);
                break;
        }
    }

    //Pour debugger depuis l'interface Unity (Code debug)

    public string testline; //DEBUG
    public void testcommand() //DEBUG
    {
        ExecuteCommand(testline);
    }


    [CustomEditor(typeof(CommandParser))]
    public class testInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CommandParser parser = (CommandParser)target;
            if(GUILayout.Button("test command"))
            {
                parser.testcommand();
            }
        }
    }
}
