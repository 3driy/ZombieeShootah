using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using XInputDotNetPure;


public class ShootTanya : MonoBehaviour
{
    //PlayerIndex playerIndex;
    //GamePadState state;
    //GamePadState prevState;
    TanyaRifleControll controllScript;
    MeleeAttack meleeScript;
    PlayerHealthAmmo ammoScript;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private float nextShoot = 0.2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private float magnitude = 0.3f;

    [SerializeField] private GameObject reloadImg;
    [SerializeField] private GameObject aimingCross;
    [SerializeField] private GameObject shotsText;
    [SerializeField] private GameObject noAmmoText;
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject meleeObject;
    [HideInInspector] public Vector3 hitPoint;
    [HideInInspector] public Vector3 nonHitPoint;
    [HideInInspector] public bool aimingForHit;
    [HideInInspector] public bool enemyHit;
    private float timer = 0f;
    private Image reloadImgItself;
    private Image crossImgItself;
    Color crossColor;
    [HideInInspector] public bool reloading;
    [HideInInspector] public int shot = 0;
    bool hitting;



    private void Start()
    {
        controllScript = GetComponent<TanyaRifleControll>();
        reloadImgItself = reloadImg.GetComponent<Image>();
        crossImgItself = aimingCross.GetComponent<Image>();
        crossColor = crossImgItself.color;
        meleeScript = GetComponentInChildren<MeleeAttack>();
        ammoScript = GetComponent<PlayerHealthAmmo>();
        reloadImg.SetActive(false);
        meleeObject.SetActive(false);
    }
    void Update()
    {

        timer += Time.deltaTime;
        bool noAmmoAtAll;
        if (ammoScript.noAmmo & shot == ammoScript.shotsBeforeReload) { noAmmoAtAll = true; }
        else { noAmmoAtAll = false; }
        if (Input.GetAxis("Fire") > 0.5f)
        {
            if (timer > nextShoot && controllScript.canShoot && shot < ammoScript.shotsBeforeReload && !reloading && !noAmmoAtAll)
            {
                Shoot();
            }
            if (controllScript.canShoot && shot == ammoScript.shotsBeforeReload && !reloading && !ammoScript.noAmmo)
            {
                reloading = true;
                StartCoroutine("Reload");
            }
            if (noAmmoAtAll) { noAmmoText.SetActive(true);
                aimingCross.SetActive(false);
            }
        }
        if (Input.GetAxis("Fire") < 0.1f || !noAmmoAtAll ) {
             noAmmoText.SetActive(false);
        }

        if (Input.GetButtonDown("Reload") & shot != 0 && !reloading && !ammoScript.noAmmo)
        {

            reloading = true;
            StartCoroutine("Reload");
  
        }
        if (Input.GetButtonDown("Hit") & !hitting)
        {
            hitting = true;
            meleeScript.listening = true;
            weaponObject.SetActive(false);
            meleeObject.SetActive(true);
            StartCoroutine("Melee");
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.SetBool("shoot", false);
        }

        
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 300f))
        {
            GameObject hitted = hit.collider.gameObject;
            if (hitted.tag == "Enemy" | hitted.tag == "EnemyHead")
            {
                crossImgItself.color = Color.red * crossColor;
            }
            else {
                crossImgItself.color = crossColor;
            }

        }
    }

    IEnumerator ShakeCam()
    {

        float fade = 1f;
        while (fade > 0f)
        {
            float xAmount = Random.Range(-1f, 1f) * magnitude * fade;
            float yAmount = Random.Range(0.4f, 1f) * magnitude * fade;
            mainCamera.transform.localEulerAngles += new Vector3(xAmount, yAmount, 0f);
            //GamePad.SetVibration(playerIndex, fade * 0.5f, fade);
            fade -= Time.deltaTime * 4f;
            yield return null;
        }
        if (fade <= 0f)
        {
            //GamePad.SetVibration(playerIndex, 0, 0);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

    }
    private void Shoot()
    {
        LayerMask finalMask =~ LayerMask.GetMask("MeleeEnemy");
        RaycastHit hit;
        Vector3 startpos = shootPoint.transform.position;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 300f, finalMask))
        {
            GameObject hitted = hit.collider.gameObject;
            aimingForHit = true;
            hitPoint = hit.point;
            enemyHit = false;
            if (hitted.tag == "Enemy")
            {
                enemyHit = true;
                ZombieController zombie = hitted.transform.root.GetComponent<ZombieController>();
                zombie.TakeDamage(Random.Range(5f, 10f));
            }
            if (hitted.tag == "EnemyHead") {
                enemyHit = true;
                ZombieController zombie = hitted.transform.root.GetComponent<ZombieController>();
                zombie.TakeDamage(Random.Range(25f, 50f));

            }
        }
        else
        {
            aimingForHit = false;
            enemyHit = false;
            nonHitPoint = mainCamera.transform.position + mainCamera.transform.TransformDirection(Vector3.forward) * 100f;
        }
        StartCoroutine("ShakeCam");
        StartCoroutine("CrossScale");
        animator.Play("AimingLayer.shoot", 1,0);
        animator.SetBool("shoot", true);
        Instantiate(muzzlePrefab, shootPoint.transform.position, shootPoint.transform.rotation, shootPoint.transform.parent);
        Instantiate(bulletPrefab, startpos, Quaternion.identity);
        shot++;
        timer = 0f;
    }

    IEnumerator Reload()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 1);
        animator.SetBool("reload", true);
        float reloader = 1.1f;
        while (reloader > 0f)
        {
            aimingCross.SetActive(false);
            if (controllScript.canShoot)
            {
                reloadImg.SetActive(true);
                float alpha = reloadImgItself.color.a;
                reloadImgItself.color = Color.Lerp(Color.red, Color.white, 1f - reloader);
                reloadImgItself.color *= new Vector4(1f, 1f, 1f, alpha);
                reloadImgItself.fillAmount = 1f - reloader;
            }
            reloader -= Time.deltaTime;
            yield return null;
        }
        if (reloader <= 0f || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("reload", false);
            if (Input.GetAxis("Aim") < 0.1f)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
            else { 
            
            aimingCross.SetActive(true);
            }
            reloadImg.SetActive(false);
            reloading = false;

            int bulletsNeeded = shot;
            if (ammoScript.ammo >= bulletsNeeded)
            {
                shot = 0;
                ammoScript.Reload(bulletsNeeded);
            }
            else
            {
                shot = shot - ammoScript.ammo;
                ammoScript.Reload(bulletsNeeded);
            }
            
            yield break;
        }
    }
    IEnumerator Melee()
    {
        //animator.SetBool("hit", true);
        animator.Play("Base Layer.hit", 0);
        float hit = 0.5f;
        while (hit > 0f)
        {

            meleeScript.listening = true;
            controllScript.hitting = true;
            aimingCross.SetActive(false);
            hit -= Time.deltaTime;
            yield return null;
        }
        if (hit <= 0f || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            controllScript.hitting = false;
            animator.SetBool("hit", false);
            if (Input.GetAxis("Aim") < 0.1f)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
            else { 
            
            aimingCross.SetActive(true);
            }
            weaponObject.SetActive(true);
            meleeObject.SetActive(false);
            meleeScript.listening = false;
            hitting = false;
            yield break;
        }
    }

    IEnumerator CrossScale()
    {

        float timer = 1f;
        while (timer > 0f)
        {

            aimingCross.transform.localScale *= Mathf.Lerp(1f, 1.7f, timer);
            timer -= Time.deltaTime * 2;
            yield return null;
        }
    }

    
}
