using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDetection : MonoBehaviour {
    public float rotateDuration;
    public float rotationDegrees;

    static private bool rotating = false;
    static public bool Rotating { get { return rotating; } }

    void OnTriggerEnter(Collider collider) {
        string tag = collider.gameObject.tag;
        Debug.Log(tag);
        if (tag == "Selectable") {
            if (!rotating) {
                Debug.Log("start rotation");
                StartCoroutine(Rotate90(collider.gameObject, new Vector3(0, 0, rotationDegrees), rotateDuration));
            }
        }
    }

    static public IEnumerator Rotate90(GameObject objectToRotate, Vector3 angles, float duration) {
        rotating = true;
        Quaternion startRotation = objectToRotate.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        for (float t = 0; t < duration; t += Time.deltaTime) {
            objectToRotate.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t / duration);

            yield return null;
        }
        objectToRotate.transform.localRotation = endRotation;
        yield return new WaitForSeconds(0.4f); // was 0.2f
        rotating = false;
    }

}
