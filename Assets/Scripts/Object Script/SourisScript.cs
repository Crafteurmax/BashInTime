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
    ConditionsManager conditionsManager;

    public void Interact()
    {
        if (conditionsManager.GetConditionState("gotCheese"))
        {
            chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
            dialogSystem.StartDialogue(dialog1, new System.Action[] { giveCheese, killMouse });
        }
    }

    private void giveCheese()
    {

    }

    private void killMouse()
    {

    }
}
