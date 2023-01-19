using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitalTrigger : MonoBehaviour
{
    public AudioSource playSound;


    IEnumerator OnTriggerEnter(Collider other)
    {
        yield return new WaitForSeconds(1f);
        playSound.Play();
    }
}
