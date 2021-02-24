using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField]CapsuleCollider capsuleCollider;
    [HideInInspector]public bool listening;

    void Update()
    {
        if (listening)
        {
            //capsuleCollider.enabled = true;
            capsuleCollider.enabled = true;
            //rigidbody.enabled = true;
        }
        else {
            capsuleCollider.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyMelee") {
            ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
            zombie.TakeMeleeDamage(Random.Range(10f,25f));
        }
    }
}
