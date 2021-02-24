using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{
    SceneControll sceneController;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerChest;
    [SerializeField] private Quaternion chestOffset;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] float camSpeed;
    [SerializeField] Transform camYaw;
    [SerializeField] Transform camPitch;
    [SerializeField] private float minRotAiming = -40;
    [SerializeField] private float maxRotAiming = 80f;
    [SerializeField] private float minRotNotAiming = -15f;
    [SerializeField] private float maxRotNotAiming = 60f;
    [SerializeField] Vector3 aimingPoint = new Vector3(0.5f, 0.5f, -1f);
    [SerializeField] private float aimingFOV = 30f;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private GameObject aimingButton;
    [SerializeField] private GameObject shootingButton;
    [SerializeField] private GameObject nonAimingButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject inGameUI;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject reloadButtonObj;
    [SerializeField] private GameObject cross;
    //Vector2 touchStartPos;
    Vector2 inputOrient;
    //Quaternion animChest;
    Color startCrossColor;
    Image crossImage;
    Camera cameraCam;
    float startFOV;
    Vector3 startCamPos;
    Vector2 inputSpeed;
    CharacterController controller;
    [HideInInspector] public Animator animator;
    bool joystickMove;
    float camDistance;
    [HideInInspector] public bool aiming;
    bool hitting;
    bool shootingButtonPressed;
    PlayerHealthAmmo ammoHealthScript;
    MeleeAttack meleeScript;
    float timer = 0f;
    bool noAmmoAtAll;
    [SerializeField] private GameObject reloadImg;
    [SerializeField] private GameObject noAmmoText;
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject meleeObject;
    [HideInInspector] public Vector3 hitPoint;
    [HideInInspector] public Vector3 nonHitPoint;
    [HideInInspector] public bool aimingForHit;
    [HideInInspector] public bool enemyHit;

    [SerializeField] private GameObject shootPoint;
    [SerializeField] public Text score;
    [SerializeField] public GameObject saveIcon;
    [SerializeField] private float nextShoot = 0.2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private float magnitude = 0.3f;
    [HideInInspector] public int shot;
    Image reloadImgItself;
    private bool reloading;
    private int rightFingerId;


    [SerializeField] AudioClip[] shootSounds;
    [SerializeField] AudioClip[] meleeSounds;
    [SerializeField] AudioClip reloadSound;
    AudioSource shootingAudioSource;
    [SerializeField] AudioClip[] stepSounds;
    AudioSource tanyaSoundSource;
    Vector3 startStepPos;
    [SerializeField] private float stepDistance = 1f;
    //[SerializeField] Texture2D lightCookie;
    Light shootlight;
    //Light flashlight;
    float startIntensity;
    private void Start()
    {
        rightFingerId = -1;
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        shootingButtonPressed = false;
        meleeScript = GetComponentInChildren<MeleeAttack>();
        ammoHealthScript = GetComponent<PlayerHealthAmmo>();
        camDistance = mainCamera.transform.localPosition.z;
        cameraCam = mainCamera.GetComponent<Camera>();
        startCamPos = mainCamera.transform.localPosition;
        startFOV = cameraCam.fieldOfView;
        crossImage = cross.GetComponent<Image>();
        aiming = false;
        nonAimingButton.SetActive(false);
        shootingButton.SetActive(false);
        controller = GetComponent<CharacterController>();
        animator = player.GetComponent<Animator>();
        startCrossColor = crossImage.color;
        reloadImgItself = reloadImg.GetComponent<Image>();
        reloadImg.SetActive(false);
        meleeObject.SetActive(false);
        shootingAudioSource = shootPoint.GetComponent<AudioSource>();
        tanyaSoundSource = GetComponent<AudioSource>();
        startStepPos = transform.position;
        shootlight = shootPoint.GetComponent<Light>();
        shootlight.enabled = false;
        startIntensity = shootlight.intensity;
        //flashlight = shootPoint.transform.GetChild(0).gameObject.GetComponent<Light>();
        //flashlight.cookie = lightCookie;
    }

    private void Update()
    {
        score.text = "YOUR SCORE: " + sceneController.killedInGame;
        if (!sceneController.paused)
        {
            Orientation();
            PlayerMove();
            AnimControll();
            Aiming();
            ShootingUpdate();
            if (!aiming)
            {
                CameraCollision();
            }
        }

        if ((transform.position - startStepPos).magnitude > stepDistance * 1f - Vector2.ClampMagnitude(new Vector2(joystick.Horizontal,joystick.Vertical),1f).magnitude)
        {
            MakeStep();
        }

    }
    void LateUpdate()
    {
        if (aiming & !animator.GetBool("reload") & !hitting)
        {
            ChestRotation();
        }
    }
    private void FixedUpdate()
    {
        GravityFall();
        Touchinput();
    }

    void Orientation()
    {
        Vector3 startRot = Touchinput();
        float camYawRot = camYaw.localRotation.eulerAngles.y;
        float camPitchRot = camPitch.localRotation.eulerAngles.x;
        float minRot = 0f;
        float maxRot = 0f;
        float localCamSpeed = camSpeed;
        if (!aiming)
        {
            minRot = minRotNotAiming;
            maxRot = maxRotNotAiming;
        }
        if (aiming)
        {
            minRot = minRotAiming;
            maxRot = maxRotAiming;
            localCamSpeed = camSpeed * 0.3f;
        }
        Vector3 minBounds = Vector3.right * minRot;
        Vector3 maxBounds = Vector3.right * maxRot;
        camYawRot = startRot.y + inputOrient.x * localCamSpeed;
        if (inputOrient.x > 0.1f || inputOrient.y > 0.1f || inputOrient.x < -0.1f || inputOrient.y < -0.1f)
        {
            camYaw.localEulerAngles = Vector3.up * camYawRot;
            camPitchRot = startRot.x - inputOrient.y * localCamSpeed;
            Quaternion pitchRot = Quaternion.Euler(Vector3.right * camPitchRot);
            camPitch.localRotation = ClampRotation(pitchRot, minBounds, maxBounds);
        }

    }
    private Quaternion ClampRotation(Quaternion q, Vector3 minBounds, Vector3 maxBounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, minBounds.x, maxBounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, minBounds.y, maxBounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, minBounds.z, maxBounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }
    void GravityFall()
    {
        float slopeMultiplier = 1f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, 1f))
        {
            slopeMultiplier = 1 - Vector3.Dot(Vector3.up, hit.normal);
        }
        Vector3 playerVelocity = Vector3.zero;
        playerVelocity.y -= slopeMultiplier * 80f * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    void PlayerMove()
    {
        inputSpeed = new Vector2(joystick.Vertical, joystick.Horizontal);
        inputSpeed = Vector2.ClampMagnitude(inputSpeed, 1f);
        if (inputSpeed.x > 0.1f | inputSpeed.y > 0.1f | inputSpeed.x < -0.1f | inputSpeed.y < -0.1f && !hitting)
        { joystickMove = true; }
        else
        { joystickMove = false; }

        if (joystickMove)
        {
            Vector3 runOrient = camYaw.transform.TransformDirection(Vector3.forward * inputSpeed.x + Vector3.right * inputSpeed.y).normalized;
            float playerYRot = camYaw.transform.localEulerAngles.y;
            Vector3 playerRot = Vector3.up * playerYRot;
            player.transform.localEulerAngles = playerRot;
            runOrient *= 3f;
            if (controller.isGrounded)
            {
                if (!aiming)
                {
                    controller.SimpleMove(runOrient);
                }
                else
                {
                    controller.SimpleMove(runOrient * 0.5f);
                }
            }

        }
    }
    void AnimControll()
    {
        if (joystickMove)
        {
            animator.SetFloat("InputVertical", inputSpeed.y);
            animator.SetFloat("InputHorizontal", inputSpeed.x);
            animator.SetBool("joystickMove", true);
            player.transform.rotation = camYaw.rotation;
        }

        else
        {
            animator.SetBool("joystickMove", false);
            float playerYRot = player.transform.localEulerAngles.y;
            Vector3 playerRot = Vector3.up * playerYRot;
            player.transform.localEulerAngles = playerRot;
        }


    }
    public void AimingButton()
    {
        aiming = !aiming;
        //aimingButton.SetActive(!aiming);
        //shootingButton.SetActive(!aiming);
        //nonAimingButton.SetActive(aiming);
        //cross.SetActive(aiming);
    }
    void Aiming()
    {
        aimingButton.SetActive(!aiming);
        shootingButton.SetActive(aiming);
        nonAimingButton.SetActive(aiming);
        cross.SetActive(aiming);

        if (aiming & !hitting)
        {

            cameraCam.fieldOfView = aimingFOV;
            mainCamera.transform.localPosition = aimingPoint;
            float playerYRot = camYaw.transform.localEulerAngles.y;
            Vector3 playerRot = Vector3.up * playerYRot;
            player.transform.localEulerAngles = playerRot;
            animator.SetBool("aiming", true);
            if (!animator.GetBool("reload"))
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 1);
            }
        }

        else
        {
            cameraCam.fieldOfView = startFOV;
            mainCamera.transform.localPosition = startCamPos;
            cross.SetActive(false);
            animator.SetBool("aiming", false);
            if (!animator.GetBool("reload"))
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
        }
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    void CameraCollision()
    {

        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Environment");
        if (Physics.Raycast(camYaw.position, mainCamera.transform.position - camYaw.position, out hit, -camDistance, mask))
        {
            Debug.DrawRay(camYaw.position, hit.point);
            Vector3 camPos = mainCamera.transform.localPosition;
            float newDistance = (camYaw.position - hit.point).magnitude;
            camPos.z = -newDistance;
            mainCamera.transform.localPosition = camPos;
        }
        else
        {
            Vector3 camPos = mainCamera.transform.localPosition;
            camPos.z = camDistance;
            mainCamera.transform.localPosition = camPos;
        }

    }
    void ChestRotation()
    {
        Quaternion targetRotation = camPitch.rotation;
        targetRotation = targetRotation * chestOffset;
        playerChest.transform.rotation = targetRotation;
    }

    Vector3 Touchinput()
    {
        Vector3 startInputRotation = new Vector3(camPitch.eulerAngles.x, camYaw.eulerAngles.y, 0);
        Vector2 lookInput = Vector3.zero;
        float thirdOfScreen = Screen.width * 0.23f;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            switch (t.phase)
            {
                case TouchPhase.Began:
                    bool overUI = IsPointerOverUIObject();
                    if (t.position.x > thirdOfScreen && rightFingerId == -1 && !overUI)
                    {
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                    }

                    break;
                case TouchPhase.Moved:

                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition;
                    }

                    break;
                case TouchPhase.Stationary:

                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }

        inputOrient = lookInput;
        return startInputRotation;

    }

    private void Shoot()
    {
        //LayerMask finalMask = ~(LayerMask.GetMask("MeleeEnemy") | LayerMask.GetMask("SavingPoint") | LayerMask.GetMask(""));
        LayerMask finalMask = ~(LayerMask.GetMask("MeleeEnemy") | LayerMask.GetMask("SavingPoint") );
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
            if (hitted.tag == "EnemyHead")
            {
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
        animator.Play("AimingLayer.shoot", 1, 0);
        animator.SetBool("shoot", true);
        shootingAudioSource.clip = shootSounds[Random.Range(0, shootSounds.Length)];
        shootingAudioSource.Play();
        Instantiate(muzzlePrefab, shootPoint.transform.position, shootPoint.transform.rotation, shootPoint.transform.parent);
        Instantiate(bulletPrefab, startpos, Quaternion.identity);
        shot++;
        timer = 0f;
    }
    IEnumerator CrossScale()
    {

        shootlight.enabled = true;
        float timer = 1f;
        
        while (timer > 0f)
        {
            shootlight.intensity = Mathf.Lerp(0, startIntensity, timer);
            cross.transform.localScale *= Mathf.Lerp(1.2f, 1f, timer);
            timer -= Time.deltaTime * 10;
            yield return null;
        }
        if (timer <= 0)
        {
            shootlight.enabled = false;
            cross.transform.localScale = Vector3.one;
            yield break;
        }
    }
    IEnumerator Melee()
    {
        tanyaSoundSource.clip = meleeSounds[Random.Range(0, meleeSounds.Length)];
        tanyaSoundSource.Play();
        //animator.SetBool("hit", true);
        animator.Play("Base Layer.hit", 0);
        float hit = 0.5f;
        //bool localAiming = aiming;
        while (hit > 0f)
        {
            aiming = false;
            meleeScript.listening = true;
            hitting = true;
            cross.SetActive(false);
            hit -= Time.deltaTime;
            yield return null;
        }
        if (hit <= 0f || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            hitting = false;
            animator.SetBool("hit", false);
            if (!aiming)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
            else
            {

                cross.SetActive(true);
            }
            //if (localAiming) { aiming = true; }
            weaponObject.SetActive(true);
            meleeObject.SetActive(false);
            meleeScript.listening = false;
            hitting = false;
            yield break;
        }
    }
    IEnumerator Reload()
    {
        shootingAudioSource.clip = reloadSound;
        shootingAudioSource.Play();
        animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 1);
        animator.SetBool("reload", true);
        float reloader = 1.1f;
        while (reloader > 0f)
        {
            cross.SetActive(false);
            if (aiming)
            {
                reloadImg.SetActive(true);
                float alpha = reloadImgItself.color.a;
                reloadButtonObj.GetComponent<Image>().fillAmount = 1f - reloader;
                reloadImgItself.color = Color.Lerp(Color.red, Color.white, 1f - reloader);
                reloadImgItself.color *= new Vector4(1f, 1f, 1f, alpha);
                reloadImgItself.fillAmount = 1f - reloader;
            }
            reloader -= Time.deltaTime;
            yield return null;
        }
        if (reloader <= 0f || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            reloadButtonObj.GetComponent<Image>().fillAmount = 1f;
            animator.SetBool("reload", false);
            if (!aiming)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
            else
            {

                cross.SetActive(true);
            }
            reloadImg.SetActive(false);
            reloading = false;

            int bulletsNeeded = shot;
            if (ammoHealthScript.ammo >= bulletsNeeded)
            {
                shot = 0;
                ammoHealthScript.Reload(bulletsNeeded);
            }
            else
            {
                shot = shot - ammoHealthScript.ammo;
                ammoHealthScript.Reload(bulletsNeeded);
            }

            yield break;
        }
    }
    IEnumerator ShakeCam()
    {

        float fade = 0.5f;
        while (fade > 0f)
        {
            float xAmount = Random.Range(-1f, 1f) * magnitude * fade;
            float yAmount = Random.Range(0.4f, 1f) * magnitude * fade;
            mainCamera.transform.localEulerAngles += new Vector3(xAmount, yAmount, 0f);
            fade -= Time.deltaTime * 4f;
            yield return null;
        }
        if (fade <= 0f)
        {
            mainCamera.transform.localRotation = Quaternion.identity;
        }

    }
    void ShootingUpdate()
    {

        timer += Time.deltaTime;
        if (ammoHealthScript.noAmmo & shot == ammoHealthScript.shotsBeforeReload) { noAmmoAtAll = true; }
        else { noAmmoAtAll = false; }
        if (shot == 0 || ammoHealthScript.noAmmo) { reloadButtonObj.GetComponent<Button>().interactable = false; }
        else { reloadButtonObj.GetComponent<Button>().interactable = true; }
        if (shootingButtonPressed)
        {
            if (timer > nextShoot && aiming && shot < ammoHealthScript.shotsBeforeReload && !reloading && !noAmmoAtAll)
            {
                Shoot();
            }
            if (aiming && shot == ammoHealthScript.shotsBeforeReload && !reloading && !ammoHealthScript.noAmmo)
            {
                reloading = true;
                StartCoroutine("Reload");
            }
            if (noAmmoAtAll)
            {
                noAmmoText.SetActive(true);
                cross.SetActive(false);
            }
        }
        if (!shootingButtonPressed || !noAmmoAtAll)
        {
            noAmmoText.SetActive(false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.SetBool("shoot", false);
        }

        LayerMask finalMask = ~(LayerMask.GetMask("MeleeEnemy") | LayerMask.GetMask("SavingPoint"));
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 300f, finalMask))
        {
            GameObject hitted = hit.collider.gameObject;
            if (hitted.tag == "Enemy" | hitted.tag == "EnemyHead")
            {
                crossImage.color = Color.red * startCrossColor;
            }
            else
            {
                crossImage.color = startCrossColor;
            }

        }

    }

    public void ShootButtonDown()
    {
        shootingButtonPressed = true;
    }
    public void ShootButtonUp()
    {
        shootingButtonPressed = false;
    }

    public void ReloadButton()
    {
        if (shot != 0 && !reloading && !ammoHealthScript.noAmmo)
        {
            reloading = true;
            StartCoroutine("Reload");

        }
    }
    public void MeleeButton()
    {
        if (!hitting)
        {
            hitting = true;
            meleeScript.listening = true;
            weaponObject.SetActive(false);
            meleeObject.SetActive(true);
            StartCoroutine("Melee");
        }
    }

    public void PauseUnPause()
    {
        if (sceneController.paused)
        {
            sceneController.UnPause();
            pauseMenu.SetActive(false);
            inGameUI.SetActive(true);
        }
        else
        {
            sceneController.Pause();
            pauseMenu.SetActive(true);
            inGameUI.SetActive(false);
        }

    }
    public void MenuButtonPressed()
    {

        sceneController.GoToMainMenu();
    }

    public void Death()
    {
        sceneController.died = true;
        animator.SetLayerWeight(0, 1f);
        weaponObject.SetActive(false);
        meleeObject.SetActive(false);
        animator.SetBool("dead", true);
    }



    public IEnumerator Damage()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 1f);
        animator.SetBool("damaged", true);
        animator.Play("hitReaction");
        float timer = 0.5f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer <= 0)
        {
            animator.SetBool("damaged", false);
            if (!aiming)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0f);
            }
            yield break;
        }


    }

    void MakeStep() {
        startStepPos = transform.position;
        tanyaSoundSource.clip = stepSounds[Random.Range(0, stepSounds.Length)];
        tanyaSoundSource.Play();
    }
}
