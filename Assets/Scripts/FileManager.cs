using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [SerializeField] private string fileRoot = "BashWork/root";
    [SerializeField] private string fileRootDefault = "BashWork/rootDefault";


    private void Awake()
    {
        if (Directory.Exists(fileRoot))
        {
            Directory.Delete(fileRoot, true);
        }
        
        Directory.CreateDirectory(fileRoot);
        CopyFilesRecursively(fileRootDefault, fileRoot);
    }


    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

}
