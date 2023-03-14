using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonOrdiScript: MonoBehaviour
{

    [SerializeField]
    ChefDorchestre chef;
    public void Interact()
    {
        chef.SwitchSystem(ChefDorchestre.GameSystem.Console);
    }


}
