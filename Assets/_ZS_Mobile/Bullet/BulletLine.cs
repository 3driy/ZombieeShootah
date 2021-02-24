using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLine : MonoBehaviour
{
    LineRenderer line;
    PlayerControll shootScript;
    [SerializeField] private GameObject burst;
    [SerializeField] private GameObject bloodBurst;
    Color color1;
    Color color2;
    private float colorTimer;
    void Awake()
    {
        shootScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControll>();
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, transform.position);
        if (shootScript.aimingForHit)
        {
            line.SetPosition(1, shootScript.hitPoint);
        }
        else { 
            line.SetPosition(1, shootScript.nonHitPoint);
        }
        color1 = line.startColor;
        color2 = line.endColor;
        line.enabled = true;
        if (shootScript.aimingForHit)
        {
            if (shootScript.enemyHit) { Instantiate(bloodBurst, shootScript.hitPoint, Quaternion.identity); }
            else
            {
                Instantiate(burst, shootScript.hitPoint, Quaternion.identity);
            }
        }
        colorTimer = color1.a;
    }
    private void Update()
    {
        DestroyEverything();
    }

    void DestroyEverything() {

      
        if (colorTimer > 0f)
        {
            line.material.color =  Color.white * (1 - colorTimer) * 4f;
            colorTimer -= Time.deltaTime * 2f;
            color1.a = color2.a = colorTimer;
            line.startColor = color1;
            line.endColor = color2;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

