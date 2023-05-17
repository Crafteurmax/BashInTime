using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorScript : MonoBehaviour
{

    [SerializeField] ChefDorchestre chef;
    [SerializeField] float duration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [SerializeField]
    ConditionsManager conditionsManager;
    public void Interact()
    {
        Debug.Log("Open the DOOOOOOOR");
        if(!conditionsManager.GetConditionState("doorIsUnlock"))
        { 
            chef.RestartSceneDelay(duration);
            chef.SwitchSystem(ChefDorchestre.GameSystem.EndOfTheWorld);
        }
        else
        {
            Debug.Log("win");
        }

       
    }

}
