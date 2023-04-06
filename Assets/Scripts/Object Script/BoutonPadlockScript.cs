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
        if (!conditionsManager.GetConditionState("Cond0")) chef.SwitchSystem(ChefDorchestre.GameSystem.Lock);
        else dialogSystem.StartDialogue(dialog, null);
    }
}
