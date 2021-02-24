using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEmitter : MonoBehaviour
{
    ParticleSystem emitter;
    void Awake()
    {
        emitter = GetComponent<ParticleSystem>();
        emitter.Stop();
        var main = emitter.main;
        float totalDuration = main.duration;
        totalDuration += main.startLifetime.constantMax;
        emitter.Play();
        Destroy(this.gameObject, totalDuration);
    }

    
}
