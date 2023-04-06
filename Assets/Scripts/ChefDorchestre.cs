using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChefDorchestre : MonoBehaviour
{
    [Serializable]
    public class SystemObjects
    {
        public string name;
        public MonoBehaviour[] components;
        public GameObject[] objects;
        public GameSystem parent;
        public bool escapeAllowed;
    }

    private SystemObjects currentSystem;

    [SerializeField] private SystemObjects[] systems;
    public PalaisMental palais;
    

    public enum GameSystem : int
    {
        Default = 0,
        Dialogue = 1,
        Console = 2,
        PalaisMental = 3,
        Lock = 4,
        EndOfTheWorld = 5,
    }

    private void Awake()
    {
        //Desactive les systems si pas mis en jeu
        foreach (SystemObjects gog in systems)
        {
            foreach (MonoBehaviour go in gog.components)
            {
                go.enabled = false;
            }

            foreach (GameObject go in gog.objects)
            {
                go.SetActive(false);
            }
        }

        currentSystem = systems[(int)GameSystem.Default];

        //On active le nouveau systeme
        foreach (MonoBehaviour go in currentSystem.components)
        {
            go.enabled = true;
        }
        foreach (GameObject go in currentSystem.objects)
        {
            go.SetActive(true);
        }
    }

    public void OpenPalaisMental()
    {
        SwitchSystem(GameSystem.PalaisMental);
    }

    public void SwitchSystem(GameSystem system)
    {
        //On desactive le systeme precedent
        foreach(MonoBehaviour go in currentSystem.components)
        {
            go.enabled = false;
        }
        foreach (GameObject go in currentSystem.objects)
        {
            go.SetActive(false);
        }

        Debug.Log(systems.Length);

        //Switch du systeme actuel
        currentSystem = systems[(int)system];

        //On active le nouveau systeme
        foreach (MonoBehaviour go in currentSystem.components)
        {
            go.enabled = true;
        }
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

    public void RestartSceneDelay(float duration)
    {
        StartCoroutine(_RestartSceneDelay(duration));
    }

    private IEnumerator _RestartSceneDelay(float duration)
    {
        float beginTime = Time.time;

        while (Time.time < beginTime + duration) yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
