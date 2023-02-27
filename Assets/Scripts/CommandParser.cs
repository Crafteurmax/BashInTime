using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public string testline;


    private CommandExecuter executer;
    private void Awake()
    {
        executer = GetComponent<CommandExecuter>();
    }

    public void testcommand()
    {
        Debug.Log(ExecuteCommand(testline));
    }

    string ExecuteCommand(string line)
    {
        line = line.Trim();

        //Illegal characters
        foreach(char c in line)
        {
            if (!authorizedCharacters.Contains(c))
            {
                return "Illegal Character : '" + c + "'";
            }
        }


        //Split words
        string[] words = line.Split(" ");

        if (words.Length <= 0) return "Please Input something";

        string command = words[0].Trim();
        CommandType type = GetCommandType(command);

        switch (type)
        {
            case CommandType.Direct:
                return RawExecute(line);
        }

        return command + " of type " + type + " executed!";
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
    private string RawExecute(string line)
    {
        return executer.Execute(line, null);
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
