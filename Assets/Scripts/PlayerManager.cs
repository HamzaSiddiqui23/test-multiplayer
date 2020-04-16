using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PlayerInputActions inputActions;
    Vector2 movementInput;
    Vector2 lookInput;
    public float moveVelocity;
    public float runVelocity;
    public float lookVelocity;
    public float jumpPower;
    public float minRotation = -23;
    public float maxRotation = 23;
    public float bulletSpeed;
    public float lerpSpeed = 3;
    Rigidbody body;
    Quaternion bodyRotation;
    Quaternion cameraRotation;
    float yRotation;
    float xRotation;
    bool jumpInput = false;
    bool isGrounded;
    public TextMeshProUGUI playerName;
    public Transform playerCamera;
    public Transform gunPoint;
    public float gunDamage = 10f;
    public float gunRange = 100f;
    public float health = 50f;
    bool isDead;
    public Slider healthSlider;
    public TextMeshProUGUI youdead;
    public ParticleSystem muzzleFlash;
    public LineRenderer bulletLine;
    public GameObject bullet;
    bool isRunning = false;
    bool crouch = false;
    Animator anim;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)
        {
            anim = GetComponent<Animator>();
            bulletLine = GetComponent<LineRenderer>();
            healthSlider.maxValue = health;
            healthSlider.value = health;
            isDead = false;
            inputActions = new PlayerInputActions();
            inputActions.PlayerActions.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Sprint.performed += ctx => isRunning = true;
            inputActions.PlayerActions.Sprint.canceled += ctx => isRunning = false;
            inputActions.PlayerActions.Crouch.performed += ctx => CrouchPressed();
            inputActions.PlayerActions.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Look.canceled += ctx => lookInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Jump.canceled += ctx => jumpInput = true;
            inputActions.PlayerActions.Fire.canceled += ctx => FireGun();
            body = GetComponent<Rigidbody>();
            isGrounded = false;
            bodyRotation = transform.rotation;
            cameraRotation = playerCamera.transform.localRotation;
        }
        if (!photonView.IsMine)
            playerCamera.gameObject.SetActive(false);
    }

    void Start()
    {
        playerName.text = photonView.Owner.NickName;
        Cursor.lockState = CursorLockMode.Locked;
        crouch = false;
    }

    public override void OnEnable()
    {
        if (photonView.IsMine)
            inputActions.Enable();
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
            inputActions.Disable();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (!isDead)
        {
            body.velocity = new Vector3(0, body.velocity.y, 0);

            yRotation = transform.rotation.eulerAngles.y;
            yRotation += lookVelocity * lookInput.x;
            bodyRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation, transform.rotation.z);

            xRotation = playerCamera.localRotation.eulerAngles.x;
            xRotation -= lookVelocity * lookInput.y;
            xRotation = (xRotation + 180f) % 360 - 180;
            cameraRotation = Quaternion.Euler(Mathf.Clamp(xRotation, minRotation, maxRotation), playerCamera.transform.localRotation.eulerAngles.y, playerCamera.transform.localRotation.eulerAngles.z);


            //transform.position += ((transform.forward * movementInput.y) + (transform.right * movementInput.x)) * moveVelocity * Time.deltaTime;
            if (!isRunning && !crouch)
            {
                body.MovePosition(transform.position + ((transform.forward * movementInput.y) + (transform.right * movementInput.x)) * moveVelocity * Time.deltaTime);
                if (movementInput != Vector2.zero)
                {
                        ClearAnimBools();
                        anim.SetBool("Walk", true);
                }
            }
            else if (!crouch && isRunning)
            {
                if (movementInput != Vector2.zero)
                {
                    body.MovePosition(transform.position + ((transform.forward * movementInput.y) + (transform.right * movementInput.x)) * runVelocity * Time.deltaTime);
                    ClearAnimBools();
                    anim.SetBool("Run", true);
                }
            }
            else
            {
                if (movementInput != Vector2.zero)
                {
                    body.MovePosition(transform.position + ((transform.forward * movementInput.y) + (transform.right * movementInput.x)) * moveVelocity * Time.deltaTime);
                    ClearAnimBools();
                    anim.SetBool("Crouch Walk", true);
                }
                else
                {
                    anim.SetBool("Crouch Walk", false);
                    anim.SetBool("Crouch", true);
                }
            }

            //body.AddForce(transform.right * movementInput.x * moveVelocity * 1000 * Time.deltaTime);

            if (isGrounded && jumpInput)
            {
                ClearAnimBools();
                anim.SetBool("Jump", true);
                body.AddForce(transform.up * jumpPower * 1000 * Time.deltaTime);
                isGrounded = false;
            }


            if (movementInput == Vector2.zero && !crouch)
            {
                    ClearAnimBools();
                    anim.SetBool("Idle", true);
            }
            jumpInput = false;

            transform.rotation = Quaternion.Lerp(transform.rotation, bodyRotation, lerpSpeed);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, cameraRotation, lerpSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && photonView.IsMine)
        {
            isGrounded = true;
        }
    }

    void FireGun()
    {
        if (!photonView.IsMine)
            return;
        StartCoroutine(ShotEffect());
        muzzleFlash.Play();
        bulletLine.SetPosition(0, gunPoint.position);
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, gunRange))
        {
            bulletLine.SetPosition(1, hit.point);
            Debug.Log(hit.collider.gameObject.name);
            PlayerManager pm = hit.transform.GetComponent<PlayerManager>();
            if(pm!=null && pm!=this)
                pm.TakeDamage(gunDamage, playerName.text);
        }
        else
        {
            bulletLine.SetPosition(1, gunPoint.transform.position + (playerCamera.transform.forward * gunRange));
        }
    }

    public void TakeDamage(float damageAmount, string shooterName)
    {
        if (!photonView.IsMine)
            return;
        health -= damageAmount;
        healthSlider.value -= damageAmount;
        if (health <= 0)
            Die(shooterName);
    }

    void Die(string shooterName)
    {
        if (!photonView.IsMine)
            return;
        isDead = true;
        youdead.text = "YOU WERE KILLED BY "+ shooterName;
        ClearAnimBools();
        anim.SetBool("Dead", true);
    }

    IEnumerator ShotEffect()
    {
        if (!photonView.IsMine)
            yield return null;
        bulletLine.enabled = true;
        yield return new WaitForSeconds(0.1f);
        bulletLine.enabled = false;
    }

    void CrouchPressed()
    {
        if (!photonView.IsMine)
            return;
        if (crouch)
        {
            crouch = false;
            anim.SetBool("Crouch",false);
            anim.SetBool("Idle", true);
        }
        else
        {
            ClearAnimBools();
            anim.SetBool("Crouch", true);
            crouch = true;
        }
    }

    void ClearAnimBools()
    {
        if (!photonView.IsMine)
            return;
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }
}
