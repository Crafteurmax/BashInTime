using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CommandParser : MonoBehaviour
{

    private string authorizedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 \n\r\t/\\";

    public enum CommandType : int
    {
        Error = -1,
        Direct = 0,
        File = 1
    }

    public string[] fileCommands = {"cd", "ls", "rm", "grep", "mkdir", "cat", "touch", "cp", "find", "mv", "head", "tail"};
    public string[] directCommands = { "echo", "pwd"};

    public string testline; //DEBUG


    private CommandExecuter executer;
    private void Awake()
    {
        executer = GetComponent<CommandExecuter>();
    }


    public void testcommand() //DEBUG
    {
        ExecuteCommand(testline);
    }


    public void ShowReturnValue(string returnValue)
    {
        Debug.Log(returnValue); //A changer
    }


    void ExecuteCommand(string line)
    {
        line = line.Trim();

        //Illegal characters
        foreach(char c in line)
        {
            if (!authorizedCharacters.Contains(c))
            {
                ShowReturnValue("Illegal Character : '" + c + "'");
            }
        }


        //Split words
        string[] words = line.Split(" ");

        if (words.Length <= 0) ShowReturnValue("Please Input something");

        string command = words[0].Trim();
        CommandType type = GetCommandType(command);

        switch (type)
        {
            case CommandType.Error:
                ShowReturnValue(command + " of type " + type + " executed!");
                break;
            case CommandType.Direct:
                RawExecute(line, ShowReturnValue);
                break;
            case CommandType.File:
                //ShowReturnValue(RawExecute(line)); ATTENTION
                break;
        }

        return;
    }


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

        return CommandType.Error;
    }


    //ATTENTION A CETTE FONCTION!!!
    private void RawExecute(string line, Action<string> callback)
    {
        StartCoroutine(executer.Execute(line, null, callback));

        return;
    }

    //DEBUG

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
