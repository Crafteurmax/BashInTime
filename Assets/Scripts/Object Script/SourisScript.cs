using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourisScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    TextAsset dialog1;

    [SerializeField]
    TextAsset dialog2;

    [SerializeField]
    ConditionsManager conditionsManager;

    [SerializeField]
    GameObject cheese;

    private bool isAlive = true;
    public void Interact()
    {
        if (isAlive)
        {
            chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
            if (conditionsManager.GetConditionState("gotCheese"))
            {
                dialogSystem.StartDialogue(dialog1, new System.Action[] { giveCheese, killMouse });
            }
            else
            {
                dialogSystem.StartDialogue(dialog2, new System.Action[] { giveCheese, killMouse });
            }
        }
        
    }

    private void giveCheese()
    {
        conditionsManager.SetCondition("gotCheese", false); 
        conditionsManager.SetCondition("doorIsUnlock", true);
        cheese.SetActive(false);
    }

    private void killMouse()
    {
        transform.GetComponent<SpriteRenderer>().flipY = true;
        transform.GetComponent<Souris>().isMoving = false;
        isAlive = false;
    }
}
