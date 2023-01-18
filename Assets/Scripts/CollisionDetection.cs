using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to detect and keep track of when a tetris block is touching the goal position
public class CollisionDetection : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {   
        // check if it is the correct piece
        if (collider.gameObject.name == "PurplePiece")
        {
            // check if the piece is at the correct angle
            Quaternion goalRotation = Quaternion.Euler(transform.eulerAngles);
            Quaternion colliderObjectRotation = Quaternion.Euler(collider.gameObject.transform.eulerAngles);
            float angle = Quaternion.Angle(goalRotation, colliderObjectRotation);
            bool sameRotation = Mathf.Abs(angle) < 1e-3f;

            if (sameRotation)
            {
                collider.gameObject.tag = "Goal";
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Goal")
        {
            collider.gameObject.tag = "Selectable";
        }
    }
}
