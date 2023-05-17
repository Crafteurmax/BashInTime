using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostItScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    TextAsset dialog;
    
    public void Interact()
    {
        PalaisMental.current.AddMemory(0);
        chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
        dialogSystem.StartDialogue(dialog, null);
    }

}
