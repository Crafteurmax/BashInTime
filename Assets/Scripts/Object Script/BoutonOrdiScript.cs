using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonOrdiScript: MonoBehaviour
{
    public bool isPoweredOn = false;

    [SerializeField]
    ChefDorchestre chef;

    [SerializeField] private AudioSource cannotPlaySound;
    
    
    public void Interact()
    {
        if (isPoweredOn)
        {
            PalaisMental.current.AddMemory(2);
            chef.SwitchSystem(ChefDorchestre.GameSystem.Console);
        }
        else cannotPlaySound.Play();
    }


}
