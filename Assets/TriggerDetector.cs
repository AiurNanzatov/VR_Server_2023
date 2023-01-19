using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public AudioSource playSound;

    public GameObject captcha_alpha;
    public GameObject captcha_alpha2;
    public GameObject captcha_alpha3;
    public GameObject captcha_alpha4;

    IEnumerator OnTriggerEnter(Collider other)
    {
        yield return new WaitForSeconds(1f);
        playSound.Play();
        /*Color tmp = captcha_alpha.GetComponent<SpriteRenderer>().material.color;
        tmp.a = 255f;
        captcha_alpha.GetComponent<SpriteRenderer>().material.color = tmp;*/
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
        }
       
    }

    IEnumerator FadeImage(bool fadeAway, string objectName)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                captcha_alpha.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            if (objectName == "captcha")
            {
                // loop over 1 second
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    captcha_alpha.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 255 * i);
                    yield return null;
                }
            }
            else if (objectName == "captcha2")
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    captcha_alpha2.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 255 * i);
                    yield return null;
                }
            }
            else if (objectName == "captcha3")
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    captcha_alpha3.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 255 * i);
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    captcha_alpha4.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 255 * i);
                    yield return null;
                }
            }

        }
    }
}
