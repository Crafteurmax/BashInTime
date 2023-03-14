using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefDorchestre : MonoBehaviour
{
    [Serializable]
    public class SystemObjects
    {
        public string name;
        public GameObject[] objects;
        public GameSystem parent;
        public bool escapeAllowed;
    }

    private SystemObjects currentSystem;

    [SerializeField] private SystemObjects[] systems;

    

    public enum GameSystem : int
    {
        Default = 0,
        Dialogue = 1,
        Console = 2
    }

    private void Start()
    {
        //Desactive les systems si pas mis en jeu
        foreach (SystemObjects gog in systems)
        {
            foreach (GameObject go in gog.objects)
            {
                go.SetActive(false);
            }
        }

        currentSystem = systems[(int)GameSystem.Default];

        //On active le nouveau systeme
        foreach (GameObject go in currentSystem.objects)
        {
            go.SetActive(true);
        }

    }

    public void SwitchSystem(GameSystem system)
    {
        //On desactive le systeme precedent
        foreach(GameObject go in currentSystem.objects)
        {
            go.SetActive(false);
        }
        
        //Switch du systeme actuel
        currentSystem = systems[(int)system];

        //On active le nouveau systeme
        foreach (GameObject go in currentSystem.objects)
        {
            go.SetActive(true);
        }
    }

    public void OnEscape()
    {
        if (currentSystem.escapeAllowed) GetOut();
    }

    public void GetOut()
    {
        SwitchSystem(currentSystem.parent);
    }
}
