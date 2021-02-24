using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthAmmo : MonoBehaviour
{
    //ShootTanya shotScript;
    PlayerControll shotScript;
    [SerializeField] public float maxPlayerHealth = 100f;
    [SerializeField] GameObject healthImgObj;
    [SerializeField] GameObject healthImgBack;
    [SerializeField] GameObject damageBack;
    [SerializeField] GameObject deathNote;
    [SerializeField] GameObject loseNote;
    [SerializeField] Text[] deathScore;
    [SerializeField] GameObject inGameUI;
    [HideInInspector] public float playerHealth;
    Color startCol;
    [SerializeField] private GameObject shotsText;
    public int shotsBeforeReload = 10;
    [SerializeField] public int maxAmmo = 200;
    [HideInInspector] public int ammo;
    [SerializeField] private int startAmmo;
    private Text shotsTextComp;
    private int shotsInMag;
    [HideInInspector] public bool noAmmo;
    Image healthImg;

    [HideInInspector] public float playerHealthZeroOne;
    [HideInInspector] public float playerAmmoZeroOne;

    [SerializeField]
    Color[] healthColor;

    void Start()
    {
        healthImg = healthImgObj.GetComponent<Image>();
        startCol = healthImg.color;
        //playerHealth = maxPlayerHealth;
        shotsTextComp = shotsText.GetComponent<Text>();
        shotScript = GetComponent<PlayerControll>();
        //ammo = startAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        deathScore[0].text = shotScript.score.text;
        deathScore[1].text = shotScript.score.text;
        deathScore[2].text = shotScript.score.text;
        ShowShots();
        deathNote.SetActive(false);
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
        playerHealth = Mathf.Clamp(playerHealth, 0, maxPlayerHealth);
        if (playerHealth <= 0)
        {
            Death();
        }
        if (ammo != 0) { noAmmo = false; }
        else { noAmmo = true; }
        float zeroToOneHealth = playerHealth / maxPlayerHealth;
        Color col = Color.white;
        if (zeroToOneHealth > 0.5f) {
            col = Color.Lerp(healthColor[1], healthColor[0], zeroToOneHealth);
        
        }
        else { 
            col = Color.Lerp(healthColor[2], healthColor[1], zeroToOneHealth);
        
        }
        healthImg.color = startCol * col;
        healthImg.fillAmount = zeroToOneHealth;

        CalculateHealthAmmo();
    }

    public void TakeDamage(float damage)
    {
        damageBack.SetActive(true);
        StartCoroutine(shotScript.Damage());
        StartCoroutine(Damage());
        playerHealth -= damage;
       
    }

    void Death()
    {

        shotScript.Death();
        inGameUI.SetActive(false);
        deathNote.SetActive(true);
    }
    public void Lose()
    {
        shotScript.Death();
        inGameUI.SetActive(false);
        loseNote.SetActive(true);
        Destroy(this);
    }


    private void ShowShots()
    {
        shotsInMag = shotsBeforeReload - shotScript.shot;

        if (shotsInMag <= 3 || ammo < shotsBeforeReload)
        {
            float alpha = shotsTextComp.color.a;
            shotsTextComp.color = Color.red;
            shotsTextComp.color *= new Vector4(1f, 1f, 1f, alpha);
        }
        else
        {
            float alpha = shotsTextComp.color.a;
            shotsTextComp.color = Color.white;
            shotsTextComp.color *= new Vector4(1f, 1f, 1f, alpha);
        }
        shotsTextComp.text =  shotsInMag + "/" + ammo;
    }

    public void Reload(int shotsDone) {
        ammo -= shotsDone;
    }

    public void AddAmmo(int addAmmo) {
        ammo += addAmmo;
    }

    public void Heal(float heal) {
        playerHealth += heal;
    }

    IEnumerator Damage() {


        Image damageBackImg = damageBack.GetComponent<Image>();
        Image healthBack = healthImgBack.GetComponent<Image>();
        Color startDamageColor = damageBackImg.color;
        Color startBackColor = healthBack.color;
        float timer = 0.5f;
        while (timer > 0f) {

            damageBackImg.color = new Vector4(1f, 1f, 1f, timer) * startDamageColor;
            healthBack.color = Color.Lerp(startBackColor, Color.red * startBackColor, timer * 2f);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer <= 0) {
            damageBackImg.color = startDamageColor;
            healthBack.color = startBackColor;
            damageBack.SetActive(false);
            yield break;
        }

        
    }

    private void CalculateHealthAmmo() {

        playerHealthZeroOne = playerHealth / maxPlayerHealth;
        playerAmmoZeroOne = ammo / maxAmmo;
    
    }
}
