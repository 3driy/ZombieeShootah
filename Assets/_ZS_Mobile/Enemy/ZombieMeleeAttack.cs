using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMeleeAttack : MonoBehaviour
{
    ZombieController zControll;
    Collider colliderr;
    //[SerializeField]  GameObject zombieObject;
    [HideInInspector] public bool damaged;
    // Start is called before the first frame update
    void Awake()
    {
        //zControll = zombieObject.GetComponent<ZombieController>();
        zControll = transform.root.GetComponent<ZombieController>();
        colliderr = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
       colliderr.enabled = zControll.attacking;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" & !damaged)
        {
            PlayerHealthAmmo playerScript = other.gameObject.GetComponent<PlayerHealthAmmo>();
            damaged = true;
            playerScript.TakeDamage(Random.Range(5f, 15f));
        }
    }
}
