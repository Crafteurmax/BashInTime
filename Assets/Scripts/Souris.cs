using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Souris : MonoBehaviour
{

    public float speed = 0.1f;
    public bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y);
        }
    }

    void Interact()
    {
        isMoving = !isMoving;
    }
}
