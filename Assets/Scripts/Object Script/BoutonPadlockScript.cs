using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonPadlockScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    TextAsset dialog;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    ConditionsManager conditionsManager;

    [SerializeField]
    GameObject cheese;
    public void Interact()
    {
        if (gameObject.transform.position.x < 0)
        {
            chef.SwitchSystem(ChefDorchestre.GameSystem.Pave);
            return;
        }

        if (!conditionsManager.GetConditionState("gotCheese"))
        {
           
            chef.SwitchSystem(ChefDorchestre.GameSystem.Lock);
        }
        else {
            chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
            dialogSystem.StartDialogue(dialog, null);
            cheese.SetActive(true);

        }
    }
}
