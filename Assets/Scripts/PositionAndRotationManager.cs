using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PositionAndRotationManager : MonoBehaviour
{
    // the vuforia AR camera
    [SerializeField] private Camera cam;
    // the cube created by vuforia to be displayed on top of the qr code image target
    [SerializeField] private GameObject pointer;
    // game objects placed on the top corners of the image target, used to determine when the phone is tilted
    [SerializeField] private GameObject corner1;
    [SerializeField] private GameObject corner2;

    // keeps track of the position of the cube created by vuforia in screen points
    public Vector2 cubePos;
    // keeps track of if the phone is tilted enough to trigger a rotation action
    public bool tiltRight = false;
    public bool tiltLeft = false;
    public float ydiff;
    public int tilt_threshold = 40;
    void Update()
    {
        cubePos = cam.WorldToScreenPoint(pointer.transform.position);

        // get the positions of the top corners of the image target
        Vector2 corner1Pos = cam.WorldToScreenPoint(corner1.transform.position);
        Vector2 corner2Pos = cam.WorldToScreenPoint(corner2.transform.position);
        // determine if the image is tilted based on the corner y-positions
        int MIN_DIFFERENCE = tilt_threshold; // may need to be adjusted if any game objects are moved
        ydiff = Mathf.Abs(corner1Pos.y - corner2Pos.y);
        if (corner1Pos.y < (corner2Pos.y - MIN_DIFFERENCE)) {
            tiltRight = true;
        } else if (corner1Pos.y > (corner2Pos.y + MIN_DIFFERENCE)) {
            tiltLeft = true;
        } else {
            tiltRight = false;
            tiltLeft = false;
        }
    }
}
