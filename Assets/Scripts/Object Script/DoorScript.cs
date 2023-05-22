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
        
        if (!conditionsManager.GetConditionState("doorIsUnlock"))
        {
            PalaisMental.current.AddMemory(1);
            chef.RestartSceneDelay(duration);
            chef.SwitchSystem(ChefDorchestre.GameSystem.EndOfTheWorld);
        }
        else
        {
            //Debug.Log("win");
            PalaisMental.current.AddMemory(12); //VICTOIRE
            chef.SwitchSystem(ChefDorchestre.GameSystem.GoodEnd);
        }

       
    }

}
