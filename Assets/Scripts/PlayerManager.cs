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
    public GameObject bullet;
    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
            isDead = false;
            inputActions = new PlayerInputActions();
            inputActions.PlayerActions.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
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
            body.MovePosition(transform.position + ((transform.forward * movementInput.y) + (transform.right * movementInput.x)) * moveVelocity * Time.deltaTime);
            //body.AddForce(transform.right * movementInput.x * moveVelocity * 1000 * Time.deltaTime);

            if (isGrounded && jumpInput)
            {
                body.AddForce(transform.up * jumpPower * 1000 * Time.deltaTime);
                isGrounded = false;
            }
            jumpInput = false;

            transform.rotation = Quaternion.Lerp(transform.rotation, bodyRotation, lerpSpeed);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, cameraRotation, lerpSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" && photonView.IsMine)
            isGrounded = true;
    }

    void FireGun()
    {
        muzzleFlash.Play();
        GameObject bul = PhotonNetwork.Instantiate(bullet.name, gunPoint.position, Quaternion.identity);
        bul.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * 50000 * Time.deltaTime);
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, gunRange))
        {
            Debug.Log(hit.collider.gameObject.name);
            PlayerManager pm = hit.transform.GetComponent<PlayerManager>();
            if(pm!=null)
                pm.TakeDamage(gunDamage, playerName.text);
        }
    }

    public void TakeDamage(float damageAmount, string shooterName)
    {
        health -= damageAmount;
        healthSlider.value -= damageAmount;
        if (health <= 0)
            Die(shooterName);
    }

    void Die(string shooterName)

    {
        isDead = true;
        youdead.text = "YOU WERE KILLED BY "+ shooterName;
    }
}
