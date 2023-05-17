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
    TextAsset dialog;

    public void Interact()
    {
        //new System.Action[] { coucou, coucou2 }
        chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
        dialogSystem.StartDialogue(dialog, null);
    }
}
