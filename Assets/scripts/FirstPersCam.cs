using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersCam : MonoBehaviour {
    public float mouseSensitivity = 100f;
    //public float moveSpeed = 5f;

    float xRotation = 0f;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        //mouse look around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localEulerAngles = new Vector3(xRotation, transform.localEulerAngles.y + mouseX, 0f);


        /*
        //wasd
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * moveSpeed * Time.deltaTime;
        */
    }
}
