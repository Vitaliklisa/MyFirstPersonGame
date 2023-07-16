using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    float xRot;
    float yRot;
    float xRotCurrent;
    float yRotCurrent;
    public Camera cam;
    public GameObject playerGameObject;
    public float sensivity = 5f;
    public float smoothTime = 0.1f;
    float currentVelosityX;
    float currentVelosityY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ProcessLook(Vector2 input)
    {
        xRot += Input.GetAxis("Mouse X") * sensivity;
        yRot += Input.GetAxis("Mouse Y") * sensivity;
        yRot = Mathf.Clamp(yRot, -89f, 89f);

        xRotCurrent = Mathf.SmoothDamp(xRotCurrent, xRot, ref currentVelosityX, smoothTime);
        yRotCurrent = Mathf.SmoothDamp(yRotCurrent, yRot, ref currentVelosityY, smoothTime);
        cam.transform.rotation = Quaternion.Euler(-yRotCurrent, xRotCurrent, 0f);
        playerGameObject.transform.rotation = Quaternion.Euler(0f, xRotCurrent, 0f);
    }

}



   