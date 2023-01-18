using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using RiptideNetworking;
using Vuforia;
using Image = UnityEngine.UI.Image;

public class MainLogic : MonoBehaviour
{
    // the RectTransform of the canvas and pointer image
    // used control the position of the pointer image in the canvas
    public RectTransform canvas;
    public RectTransform image;
    // the canvas the pointer image
    // used to show/hide the pointer image
    [SerializeField] private GameObject canvasObject;
    private static Image imageSprite;

    public ImageTargetBehaviour imageTarget;
    public Camera arCam;
    public Camera vrCam;
    public bool Pressed { get { return pressed; } }

    private GameObject player;
    [SerializeField] LineRenderer lineRend;
    [SerializeField] GameObject head;
    [SerializeField] GameObject playerParent;

    private PositionAndRotationManager script;
    private StatusEventHandler imageTargetScript;
    private Transform _selection;
    private RaycastHit raycastHit;

    // image target tracking status
    private ObserverBehaviour mObserverBehaviour;

    // used to keep track of if the phone screen is being pressed
    private static bool pressed = false;
    // used to keep track of the last rotation command
    private static string rotation_cmd = "";
    // keep track of the rotation state
    private enum RotationState {
        LEFT_ROTATION,
        RIGHT_ROTATION,
        STOP
    };
    private static RotationState rotationState = RotationState.STOP;

    // used to make sure only one rotation occurs for each time the user tilts the phone
    private bool alreadyRotated = false;

    private static GameObject selectedObject = null;
    private static GameObject goalObject;
    private static GameObject selectedFootprint = null;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material clickMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material purpleMaterial;
    [SerializeField] private Material lightBlueMaterial;
    [SerializeField] private Material footprintMaterial;

    private static Dictionary<string, Vector3> blockOriginalPositions = new Dictionary<string, Vector3>();
    private static Dictionary<string, Vector3> blockOriginalRotations = new Dictionary<string, Vector3>();

    private Sprite handSprite, arrowSprite;
    void Start()
    {
        script = arCam.GetComponent<PositionAndRotationManager>();
        imageTargetScript = imageTarget.GetComponent<StatusEventHandler>();
        goalObject = GameObject.FindGameObjectWithTag("Goal");
        player = GameObject.Find("Player");
        imageSprite = canvasObject.GetComponentsInChildren<UnityEngine.UI.Image>()[1];

        arrowSprite = Resources.Load("Sprites/cursor_sprite", typeof(Sprite)) as Sprite;
        handSprite = Resources.Load("Sprites/hand_sprite", typeof(Sprite)) as Sprite;

        // store the original positions and locations of the tetris blocks
        InitializeDictionaries();
    }

    void Update()
    {
        // reset values that were set during the last frame
        if (_selection != null) 
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null && raycastHit.collider.tag == "Selectable")
            {
                Material[] materials = selectionRenderer.materials;
                for (int i = 0; i < materials.Length; i++) 
                {
                    materials[i] = GetMaterial(raycastHit.collider.gameObject.name, raycastHit.collider.tag);
                }
                selectionRenderer.materials = materials;
            }
            _selection = null;
        }

        if (!imageTargetScript.TargetTracked) {
            image.anchoredPosition = new Vector2(0f, 0f);
            imageSprite.color = new Color(0.717f, 0f, 0f, 0.635f);
        } else {
            // create the raycast
            Vector2 anchoredPos;
            //Debug.Log("cube pos: " + script.cubePos.x + ", " + script.cubePos.y);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, script.cubePos, vrCam, out anchoredPos);

            // the order of the corners in the list is bottom left, top left, top right, and bottom right 
            var canvasCorners = new Vector3[4];
            canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
            // get the max x and y values for the pointer based on the canvas corners
            float maxX = canvas.rect.xMax;
            float maxY = canvas.rect.yMax;
            float minX = canvas.rect.xMin;
            float minY = canvas.rect.yMin;

            // make sure the x and y value of the pointer is within the valid range based on the min and max values
            float newX = anchoredPos.x > maxX ? maxX : anchoredPos.x;
            newX = newX < minX ? minX : newX;
            float newY = anchoredPos.y > maxY ? maxY : anchoredPos.y;
            newY = newY < minY ? minY : newY;
            // use the adjusted x and y values to update the position of the pointer
            image.anchoredPosition = new Vector2(newX, newY);
           //  image.anchoredPosition = anchoredPos;
            imageSprite.color = new Color(1f, 1f, 1f, 0.635f);
        }

        Ray ray;
        if (imageTargetScript.TargetTracked) {
            ray = new Ray(vrCam.transform.position, image.transform.position - vrCam.transform.position);
        } else {
            ray = vrCam.ScreenPointToRay(new Vector2(arCam.pixelWidth/2, arCam.pixelHeight/2));
        }
        
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.black);

       lineRend.enabled = true;
       // lineRend.transform.position = player.transform.position + new Vector3(0, 0, 1);
       lineRend.SetPosition(0, player.transform.position);
       lineRend.SetPosition(1, image.transform.position);
       // lineRend.SetPositions(new Vector3[2]{ player.transform.position, Vector3.up });

        // determine if the raycast hit a selectable object
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            var selection = hit.transform;

            if (hit.collider.tag == "Selectable") {
                imageSprite.sprite = handSprite;
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null) {
                    if (selectedObject == null) {
                        // highlight the object
                        Material[] materials = selectionRenderer.materials;
                        for (int i = 0; i < materials.Length; i++) {
                            materials[i] = highlightMaterial;
                        }
                        selectionRenderer.materials = materials;
                    }

                    // keep track of which object is selected
                    if (pressed && selectedObject == null && !hit.transform.gameObject.name.Contains("footp")) {
                        selectedObject = hit.transform.gameObject;
                    } else if (pressed && hit.transform.gameObject.name.Contains("footp")) {
                        selectedFootprint = hit.transform.gameObject;
                    }
                }
            } else {
                imageSprite.sprite = arrowSprite;
                lineRend.enabled = false;
            }
            /*else
            {
                lineRend.enabled = false;
            }*/
            // update values
            _selection = selection;
            raycastHit = hit;
        } else {
            imageSprite.sprite = arrowSprite;
            lineRend.enabled = false;
        }

        // handle moving and releasing the selected object if there is one
        if (pressed && selectedObject != null)
        {
            MoveSelectedObject(ray, hit);
            RotateSelectedObject();
        } else if (!pressed && selectedObject != null) {
            ReleaseObject();
            selectedObject = null;
        }

        // check if a footprint was clicked
        if (selectedFootprint != null) 
        {
            // move the player to the location of the footprint
            Debug.Log(selectedFootprint.name);
            Vector3 GlobalCameraPosition = vrCam.transform.position;
            Vector3 GlobalPlayerPosition = player.transform.position;
            Vector3 GlobalFootprintPosition = selectedFootprint.transform.position;
            Vector3 GlobalOffsetCameraPlayer = new Vector3(GlobalCameraPosition.x - GlobalPlayerPosition.x, 0, GlobalCameraPosition.z - GlobalPlayerPosition.z);
            Vector3 newRigPosition = new Vector3(GlobalFootprintPosition.x - GlobalOffsetCameraPlayer.x, GlobalPlayerPosition.y, GlobalFootprintPosition.z - GlobalOffsetCameraPlayer.z);
            player.transform.position = newRigPosition;

            // if necessary, rotate the player to face the direction the footprint is facing
            /* if (selectedFootprint.name == "footp1")
            {
                playerParent.transform.RotateAround(new Vector3(head.transform.position.x, 0, head.transform.position.z), Vector3.up, 270.0f);
            } else if (selectedFootprint.name == "footp2")
            {
                playerParent.transform.RotateAround(new Vector3(head.transform.position.x, 0, head.transform.position.z), Vector3.up, 90.0f);
            } */
            
            // reset the variable
            selectedFootprint = null;
        }
    }

    private void InitializeDictionaries()
    {
        // get all the selectable blocks
        GameObject redPiece = GameObject.FindGameObjectWithTag("Red");
        GameObject lightBluePiece = GameObject.FindGameObjectWithTag("LightBlue");
        GameObject yellowPiece = GameObject.FindGameObjectWithTag("Yellow");
        GameObject purplePiece = GameObject.FindGameObjectWithTag("Purple");

        // set the tags of all the blocks to selectable
        redPiece.tag = "Selectable";
        lightBluePiece.tag = "Selectable";
        yellowPiece.tag = "Selectable";
        purplePiece.tag = "Selectable";

        // get and store the original positions of all the selectable blocks
        blockOriginalPositions.Add("RedPiece", redPiece.transform.position);
        blockOriginalPositions.Add("LightBluePiece", lightBluePiece.transform.position);
        blockOriginalPositions.Add("YellowPiece", yellowPiece.transform.position);
        blockOriginalPositions.Add("PurplePiece", purplePiece.transform.position);

        // get and store the original rotations of all the selectable blocks
        blockOriginalRotations.Add("RedPiece", redPiece.transform.eulerAngles);
        blockOriginalRotations.Add("LightBluePiece", lightBluePiece.transform.eulerAngles);
        blockOriginalRotations.Add("YellowPiece", yellowPiece.transform.eulerAngles);
        blockOriginalRotations.Add("PurplePiece", purplePiece.transform.eulerAngles);
    }

    private Material GetMaterial(string objName, string tag)
    {
        if (objName == "RedPiece") {
            return redMaterial;
        } else if (objName == "YellowPiece") {
            return yellowMaterial;
        } else if (objName == "PurplePiece") {
            return purpleMaterial;
        } else if (objName == "LightBluePiece") {
            return lightBlueMaterial;
        } else if (objName.Contains("footp")) {
            return footprintMaterial;
        } else {
            return (tag == "Selectable") ? defaultMaterial : clickMaterial;
        }
    }

    private void MoveSelectedObject(Ray ray, RaycastHit hit)
    {
        // temporarily hide the pointer image
        if (imageSprite.enabled)
        {
            // imageSprite.enabled = false;
        }
        // move the selected object to the raycast position
        // Vector3 rayPoint = ray.GetPoint(8.5f);

        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(ray.origin, selectedObject.transform.position));
        // maybe in accurate, object will move a little bit
        Vector3 newPos = new Vector3(rayPoint.x, rayPoint.y, selectedObject.transform.position.z);
        selectedObject.transform.position = newPos;
    }

    private void RotateSelectedObject()
    {
        // handle object rotation
        /*
        if (script.tiltRight && !alreadyRotated) {
            // rotate right
            selectedObject.transform.rotation *= Quaternion.Euler(0, 90, 0);
            alreadyRotated = true;
        } else if (script.tiltLeft && !alreadyRotated) {
            // rotate left
            selectedObject.transform.rotation *= Quaternion.Euler(0, -90, 0);
            alreadyRotated = true;
        } else if (!script.tiltRight && !script.tiltLeft) {
            alreadyRotated = false;
        }
        */
        if (!selectedObject) return;
        if (rotationState == RotationState.LEFT_ROTATION && !RotationDetection.Rotating) {
            StartCoroutine(RotationDetection.Rotate90(selectedObject, new Vector3(0, 0, -90), 0.5f));
        } else if (rotationState == RotationState.RIGHT_ROTATION && !RotationDetection.Rotating) {
            StartCoroutine(RotationDetection.Rotate90(selectedObject, new Vector3(0, 0, 90), 0.5f));
        }
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        string messageText = message.GetString();
        Debug.Log(messageText);
        if (messageText == "Button down")
        {
            pressed = true;
        } else if (messageText == "Button up")
        {
            pressed = false;
        } else if (messageText == "left") {
            rotationState = RotationState.LEFT_ROTATION;
        } else if (messageText == "right") {
            rotationState = RotationState.RIGHT_ROTATION;
        } else if (messageText == "stop") {
            rotationState = RotationState.STOP;
        }
    }

    public static void Response(ushort toClientId, Message message)
    {
        SendFromServer();
    }

    private static void SendFromServer()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.SresponsetoC);
        message.AddString("load_CAPTCHA_scene");

        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private static void ReleaseObject()
    {
        // if the purple block and goal block are touching
        if (selectedObject.name == "PurplePiece" && selectedObject.GetComponent<Collider>().tag == "Goal") {
            // put the purple block in the position of the goal block
            selectedObject.transform.position = goalObject.transform.position;
            // hide the goal object
            goalObject.SetActive(false);
        } else {
            // put the block back to its original position and rotation
            selectedObject.transform.position = blockOriginalPositions[selectedObject.name];
            selectedObject.transform.eulerAngles = blockOriginalRotations[selectedObject.name];
        }

        // show the pointer image again
        imageSprite.enabled = true;
    }
}
