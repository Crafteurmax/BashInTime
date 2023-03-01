using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    // Update is called once per frame
    void Update()
    {
    }

    public void detectObject()
    {
        RaycastHit2D hit2D;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hits2D = Physics2D.Raycast(ray, out hit2D, 100);

        if (hits2D != null)
        {
            UnityEngine.Debug.Log("On a sélectionner qqch");
            UnityEngine.Debug.Log(hits2D);
        }
    }
}
