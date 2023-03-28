using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour
{
    public Color outlineColor;
    private Color invisibleColor = new Color(0, 0, 0, 0);

    [SerializeField] float outlineWidth;

    public void Start()
    {
        GetComponent<Renderer>().material.SetColor("_OutlineColor", invisibleColor);
        //GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 1.0f);
    }

    public void OnMouseEnter() 
    {
        GetComponent<Renderer>().material.SetColor("_OutlineColor", outlineColor);
        //GetComponent<Renderer>().material.SetFloat("_OutlineWidth", outlineWidth);
        UnityEngine.Debug.Log("ENTER");
    }

    public void OnMouseExit()
    {
        GetComponent<Renderer>().material.SetColor("_OutlineColor", invisibleColor);
        //GetComponent<Renderer>().material.SetFloat("_OutlineWidth", outlineWidth);
        UnityEngine.Debug.Log("EXIT");
    }

    public void OnMouseDown()
    {
        // On récupère le nom du script lié à l'object (format nomObject + "Script"), et on transforme ce nom en Type
        Type scriptType = Type.GetType(gameObject.name + "Script");
        
        MonoBehaviour script = GetComponent(scriptType) as MonoBehaviour;   // On récupère le script qui a été attaché à notre object
        script.SendMessage("Interact", SendMessageOptions.RequireReceiver);    // On exécute la commande start qui est la commande de base pour lancer le script lié à l'object
    }
}
