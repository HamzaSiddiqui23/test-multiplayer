﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PlayerInputActions inputActions;
    Vector2 movementInput;
    Vector2 lookInput;
    public float moveVelocity;
    public float lookVelocity;
    public float jumpPower;
    public float bulletSpeed;
    public float lerpSpeed = 3;
    Rigidbody body;
    Quaternion bodyRotation;
    float yRotation;
    bool jumpInput = false;
    bool isGrounded;
    public TextMeshProUGUI playerName;
    public Transform playerCamera;
    public Transform gunPoint;
    public GameObject bullet;
    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)
        {
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

        body.velocity = new Vector3(0, body.velocity.y, 0);

        yRotation = transform.rotation.eulerAngles.y;
        yRotation += lookVelocity * lookInput.x;
        bodyRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation, transform.rotation.z);

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" && photonView.IsMine)
            isGrounded = true;
    }

    void FireGun()
    {
        GameObject bulletFired = PhotonNetwork.Instantiate(bullet.name, gunPoint.position, Quaternion.identity);
        bulletFired.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * 1000 * Time.deltaTime);
    }
}
