using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingScript : MonoBehaviour
{
    public AudioSource playSound;


    IEnumerator OnTriggerEnter(Collider other)
    {
        yield return new WaitForSeconds(5f);
        playSound.Play();
    }
}
