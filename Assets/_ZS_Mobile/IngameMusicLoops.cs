using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMusicLoops : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]AudioClip[] loops; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudio(Random.Range(2,3),loops[Mathf.FloorToInt(Random.value * loops.Length)]));
    }

    IEnumerator PlayAudio(int times, AudioClip impact)
    {
        for (int i = 0; i < times; i++)
        {
            audioSource.PlayOneShot(impact, 0.3F);
            yield return new WaitForSeconds(impact.length);
        }
    }
}
