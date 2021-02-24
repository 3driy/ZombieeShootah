using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lazy;
using UnityEngine.UI;

public class SavingPoint : MonoBehaviour
{
    SceneControll sceneController;
    float timer; 
    [SerializeField] float timerReload = 10f;
    GameObject saveIconUI;
    [SerializeField]GameObject saveIcon;
    private void Start()
    {
        timer = timerReload;
        saveIcon.SetActive(false);
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, 10f);
        if (timer == 0f) {
            saveIcon.SetActive(true);
        }
        SaveIconTransform();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" & timer == 0f) {
            SaveSystem.SavePlayer(sceneController);
            PlayerControll controllScript = other.gameObject.GetComponent<PlayerControll>();
            PlayerHealthAmmo healthAmmoScript = other.gameObject.GetComponent<PlayerHealthAmmo>();
            sceneController.savedAmmo = healthAmmoScript.ammo;
            sceneController.savedShot = controllScript.shot;
            sceneController.savedHealth = healthAmmoScript.playerHealth;
            sceneController.savedPlayerPos = other.transform.position;
            sceneController.fromStart = false;
            //sceneController.killedInGame = 
            timer = timerReload;
            //saveIcon = other.gameObject.
            saveIconUI = controllScript.saveIcon;
            StartCoroutine(SaveIconVisible());
            saveIcon.SetActive(false);
        }

    }

    IEnumerator SaveIconVisible() {

        float timer = 2f;
        saveIconUI.SetActive(true);
        Color startColor = saveIconUI.GetComponent<Image>().color;
        while (timer > 0) {

            saveIconUI.GetComponent<Image>().color = startColor * new Color(1f, 1f, 1f, timer/2f);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer <= 0) {
            saveIconUI.GetComponent<Image>().color = startColor;
            saveIconUI.SetActive(false);
            yield break;
        }
    
    }

    private void SaveIconTransform() {

        saveIcon.transform.Rotate(Vector3.up, 1f);
        float sine = Mathf.Sin(Time.time * 2f);
        saveIcon.transform.localPosition += sine * Vector3.up * 0.005f;
        //saveIcon.GetComponent<SpriteRenderer>().color *= new Color(1f, 1f, 1f, (sine + 1.2f) * 0.3f);
    
    }
}
