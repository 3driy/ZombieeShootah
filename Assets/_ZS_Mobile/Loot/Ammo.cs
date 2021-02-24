using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private int minAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private GameObject ammoEffect;


    [SerializeField] float timeToDisappear;
    SceneControll sceneController;

    private void Awake()
    {
        timeToDisappear = 30f;
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        sceneController.ammoInGame.Add(this.gameObject);
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
            PlayerHealthAmmo ammoScript = other.gameObject.GetComponent<PlayerHealthAmmo>();
            if(ammoScript.ammo != ammoScript.maxAmmo)
            ammoScript.AddAmmo(Random.Range(minAmmo, maxAmmo));
            GameObject effect = ammoEffect;
            Instantiate(effect, other.gameObject.transform.position + Vector3.up * 0.7f, Quaternion.identity, other.gameObject.transform);
            //sceneController.ammoInGame.Remove(this.gameObject);
            DestroyLoot();
        }
    }

    private void DestroyLoot()
    {

        sceneController.ammoInGame.Remove(this.gameObject);
        Destroy(this.gameObject);

    }
}
