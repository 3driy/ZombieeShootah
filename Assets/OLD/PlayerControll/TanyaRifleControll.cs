using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TanyaRifleControll : MonoBehaviour
{
    //[SerializeField] float speed;
    [SerializeField] float camSpeed;
    [SerializeField] GameObject player;
    [SerializeField] GameObject mainCamera;
    [SerializeField] Transform camYaw;
    [SerializeField] Transform camPitch;
    [SerializeField] private float minRotAiming = -40;
    [SerializeField] private float maxRotAiming = 80f;
    [SerializeField] private float minRotNotAiming = -15f;
    [SerializeField] private float maxRotNotAiming = 60f;
    Animator animator;
    Camera cameraCam;
    bool aiming;
     bool joystickMove;
    [HideInInspector] public bool hitting;
    CharacterController controller;
    Vector2 inputSpeed;
    Color startCrossColor;

    float startFOV;
    Vector3 startPos;
    [SerializeField] Vector3 aimingPoint = new Vector3(0.5f, 0.5f, -1f);
    [SerializeField] private float aimingFOV = 30f;
    [SerializeField] private GameObject cross;
    [SerializeField] private Transform playerChest;
    [SerializeField] private Quaternion chestOffset;
    Quaternion animChest;
    private Image crossImage;
    [HideInInspector] public bool canShoot;
    [SerializeField] private float gravityValue = 30f;
    private bool running;
    float camDistance;
    void Start()
    {
        camDistance = mainCamera.transform.localPosition.z;
        cameraCam = mainCamera.GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
        startPos = mainCamera.transform.localPosition;
        startFOV = cameraCam.fieldOfView;
        animator = player.GetComponent<Animator>();
        crossImage = cross.GetComponent<Image>();
    }

    void Update()
    {
        startCrossColor = crossImage.color;
        InputSpeed();
        Orientation();
        PlayerMove();
        AnimControll();
        Aiming();
        if (!aiming)
        {
            CameraCollision();
        }
        animChest = playerChest.transform.rotation;
    }

    void LateUpdate()
    {
        LateAiming();
        if (aiming & !animator.GetBool("reload") & !hitting)
        {
            ChestRotation();
        }
    }

    private void FixedUpdate()
    {
        GravityFall();
    }
    void InputSpeed()
    {

        inputSpeed = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        inputSpeed = Vector2.ClampMagnitude(inputSpeed, 1f);
        if (inputSpeed.x > 0.1f | inputSpeed.y > 0.1f | inputSpeed.x < -0.1f | inputSpeed.y < -0.1f && !hitting)
            //if (inputSpeed.x > 0.1f | inputSpeed.y > 0.1f | inputSpeed.x < -0.1f | inputSpeed.y < -0.1f)
        { joystickMove = true; }
        else
        { joystickMove = false; }
    }
    void Orientation()
    {
        Vector2 inputOrient = new Vector2(Input.GetAxis("HorizontalR"), -Input.GetAxis("VerticalR"));
        float camYawRot = camYaw.localRotation.eulerAngles.y;
        float camPitchRot = camPitch.localRotation.eulerAngles.x;
        float minRot = 0f;
        float maxRot = 0f;
        float localCamSpeed;
        localCamSpeed = camSpeed;
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
            //pitchSpeed *= 0.1f;
            //yawSpeed *= 0.1f;
        }
        Vector3 minBounds = Vector3.right * minRot;
        Vector3 maxBounds = Vector3.right * maxRot;
        camYawRot += inputOrient.y * localCamSpeed;
        if (inputOrient.x > 0.1f || inputOrient.y > 0.1f || inputOrient.x < -0.1f || inputOrient.y < -0.1f)
        {
            camYaw.localEulerAngles = Vector3.up * camYawRot;
            camPitchRot = camPitchRot + inputOrient.x * localCamSpeed;
            Quaternion pitchRot = Quaternion.Euler(Vector3.right * camPitchRot);
            camPitch.localRotation = ClampRotation(pitchRot, minBounds, maxBounds);
        }

    }
    void PlayerMove()
    {
        if (!joystickMove) { running = false; }
        if (Input.GetAxisRaw("Run") > 0) { running = true; }

        if (joystickMove)
        {
            Vector3 runOrient = camYaw.transform.TransformDirection(Vector3.forward * inputSpeed.x + Vector3.right * inputSpeed.y).normalized;
            float playerYRot = camYaw.transform.localEulerAngles.y;
            Vector3 playerRot = Vector3.up * playerYRot;
            player.transform.localEulerAngles = playerRot;
            if (controller.isGrounded)
            {
                if (running)
                {
                    controller.SimpleMove(runOrient * 5f);
                }
                else
                {
                    controller.SimpleMove(runOrient * 2.5f);
                }
            }

        }
    }
    void Aiming()
    {
        float aim = Input.GetAxis("Aim");
        float colorAlpha = startCrossColor.a * Mathf.Lerp(0f, 1f, aim);
        crossImage.color = new Vector4(startCrossColor.r, startCrossColor.g, startCrossColor.b, colorAlpha);

        if (aim > 0.01f & !hitting)
        {
            cross.SetActive(true);
            running = false;
            cross.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.4f, aim);
            Vector3 camPos = Vector3.Lerp(startPos, aimingPoint, aim);
            cameraCam.fieldOfView = Mathf.Lerp(startFOV, aimingFOV, aim);
            mainCamera.transform.localPosition = camPos;
            float playerYRot = camYaw.transform.localEulerAngles.y;
            Vector3 playerRot = Vector3.up * playerYRot;
            player.transform.localEulerAngles = playerRot;
            aiming = true;
        }

        else
        {
            cameraCam.fieldOfView = startFOV;
            mainCamera.transform.localPosition = startPos;
            cross.SetActive(false);
            aiming = false;
        }

        if (aim > 0.5f) { canShoot = true; }
        if (aim < 0.5f) { canShoot = false; }
    }
    void LateAiming()
    {
        float aim = Input.GetAxis("Aim");
     
        if (aim > 0.01f & !hitting)
        {
            animator.SetBool("aiming", true);
            if (!animator.GetBool("reload"))
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), aim);
            }
        }

        else
        {
            animator.SetBool("aiming", false);
            if (!animator.GetBool("reload"))
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimingLayer"), 0);
            }
        }
    }
    void AnimControll()
    {
        if (joystickMove)
        {
            float runner = 0.5f;
            if (running) { runner = 1f; }
            animator.SetFloat("InputVertical", inputSpeed.y * runner);
            animator.SetFloat("InputHorizontal", inputSpeed.x * runner);
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
    void ChestRotation()
    {

        Quaternion targetRotation = camPitch.rotation;
        targetRotation = targetRotation * chestOffset;

        playerChest.transform.rotation = Quaternion.Lerp(animChest, targetRotation, Input.GetAxis("Aim"));
        //playerChest.transform.rotation = targetRotation;

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
            playerVelocity.y -= slopeMultiplier * gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
    }

    void CameraCollision() {

        RaycastHit hit;
        //if (Physics.Raycast(camYaw.position, mainCamera.transform.position, out hit, camDistance))
        LayerMask mask = LayerMask.GetMask("Environment");
        if (Physics.Raycast(camYaw.position, mainCamera.transform.position, out hit, Mathf.Abs(camDistance), mask))
        {
            Vector3 camPos = mainCamera.transform.localPosition;
            float newDistance = (camYaw.position - hit.point).magnitude;
            camPos.z = -newDistance;
            mainCamera.transform.localPosition = camPos;
        }
        else {
            Vector3 camPos = mainCamera.transform.localPosition;
            camPos.z = camDistance;
            mainCamera.transform.localPosition = camPos;
        }

    }
}
