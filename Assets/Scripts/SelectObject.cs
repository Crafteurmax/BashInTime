using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour
{


    public void OnMouseEnter() 
    {
        //UnityEngine.Debug.Log("ENTER");
    }

    public void OnMouseExit()
    {
        //UnityEngine.Debug.Log("EXIT");
    }

    public void OnMouseDown()
    {
        // On r�cup�re le nom du script li� � l'object (format nomObject + "Script"), et on transforme ce nom en Type
        Type scriptType = Type.GetType(gameObject.name + "Script");
        
        MonoBehaviour script = GetComponent(scriptType) as MonoBehaviour;   // On r�cup�re le script qui a �t� attach� � notre object
        script.SendMessage("start", SendMessageOptions.RequireReceiver);    // On ex�cute la commande start qui est la commande de base pour lancer le script li� � l'object
    }



    public void test()
    {
        UnityEngine.Debug.Log("test");
    }
}
