using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSequenceLauncher : MonoBehaviour
{
    public AudioSource playSoundEnter;
    public AudioSource playSound1;
    public AudioSource playSound2;
    public AudioSource playSound3;


    IEnumerator OnTriggerEnter(Collider other)
    {
        yield return new WaitForSeconds(1f);
        playSoundEnter.Play();

        yield return new WaitForSeconds(12f);
        playSound1.Play();

        yield return new WaitForSeconds(4f);
        playSound2.Play();

        yield return new WaitForSeconds(4f);
        playSound3.Play();
        /*Color tmp = captcha_alpha.GetComponent<SpriteRenderer>().material.color;
        tmp.a = 255f;
        captcha_alpha.GetComponent<SpriteRenderer>().material.color = tmp;
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                StartCoroutine(FadeImage(false, "captcha"));
                break;

            case 1:
                StartCoroutine(FadeImage(false, "captcha2"));
                break;

            case 2:
                StartCoroutine(FadeImage(false, "captcha3"));
                break;

            case 3:
                StartCoroutine(FadeImage(false, "captcha4"));
                break;
            case 4:
                StartCoroutine(FadeImage(false, "captcha5"));
                break;
        }*/

    }
}
