using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Souris : MonoBehaviour
{

    public float speed = -0.01f;
    public bool isMoving = false;



    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if (transform.position.x >= 6) {
                speed = -speed;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            } else if (transform.position.x <= -5)
            {
                speed = -speed;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }

            transform.position = new Vector3(transform.position.x + speed, transform.position.y);
        }
        }

    void Interact()
    {
        isMoving = !isMoving;
    }
}
