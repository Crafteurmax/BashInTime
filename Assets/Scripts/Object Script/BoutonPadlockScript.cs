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
    public void Interact()
    {


        if (!conditionsManager.GetConditionState("Cond0"))
        {
            if (gameObject.transform.position.x < 0)
            {
                chef.SwitchSystem(ChefDorchestre.GameSystem.Pave);
                return;
            }
            chef.SwitchSystem(ChefDorchestre.GameSystem.Lock);
        }
        else dialogSystem.StartDialogue(dialog, null);
    }
}
