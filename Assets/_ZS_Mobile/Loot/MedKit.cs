using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : MonoBehaviour
{
    [SerializeField] private float minHeal;
    [SerializeField] private float maxHeal;
    [SerializeField] private GameObject healEffect;


    [SerializeField] float timeToDisappear;
    SceneControll sceneController;

    private void Awake()
    {
        timeToDisappear = 30f;
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        sceneController.medkitsInGame.Add(this.gameObject);
    }

    private void Update()
    {
        timeToDisappear -= Time.deltaTime;
        if (timeToDisappear <= 0)
        {
            DestroyLoot();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerHealthAmmo healthScript = other.gameObject.GetComponent<PlayerHealthAmmo>();

            if (healthScript.playerHealth != healthScript.maxPlayerHealth)
            {
                healthScript.Heal(Random.Range(minHeal, maxHeal));
                GameObject effect = healEffect;
                Instantiate(effect, other.gameObject.transform.position, Quaternion.identity, other.gameObject.transform);
                DestroyLoot();
            }
            else {
                healthScript.Heal(0);
            }
        }
    }

    private void DestroyLoot() {

        sceneController.medkitsInGame.Remove(this.gameObject);
                Destroy(this.gameObject);
    
    }
}
