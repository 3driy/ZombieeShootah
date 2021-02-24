using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMeshPicker : MonoBehaviour
{
    [SerializeField] Color[] colorsToPick;
    [SerializeField] GameObject[] zombieLegs;
    [SerializeField] GameObject[] zombieTorsos;
    [SerializeField] GameObject[] zombieHeads;

    // Start is called before the first frame update
    void Start()
    {
        int randomLegs = Random.Range(0,zombieLegs.Length);
        for (int i = 0; i < zombieLegs.Length; i++)
        {
            if (i != randomLegs)
            {
                DestroyImmediate(zombieLegs[i]);
            }
            else {
                Renderer Rend = zombieLegs[i].GetComponent<SkinnedMeshRenderer>();
                Rend.materials[1].color = colorsToPick[Random.Range(0, colorsToPick.Length)];
            
            }
        }
        int randomTorso = Random.Range(0,zombieTorsos.Length);
        for (int i = 0; i < zombieTorsos.Length; i++)
        {
            if (i != randomTorso)
            {
                DestroyImmediate(zombieTorsos[i]);
            }
            else
            {
                Renderer Rend = zombieTorsos[i].GetComponent<SkinnedMeshRenderer>();
                Rend.materials[1].color = colorsToPick[Random.Range(0, colorsToPick.Length)];

            }
        }
        int randomHead = Random.Range(0, zombieHeads.Length);
        for (int i = 0; i < zombieHeads.Length; i++)
        {
            if (i != randomHead)
            {
                DestroyImmediate(zombieHeads[i]);
            }

        }




    }

  
}
