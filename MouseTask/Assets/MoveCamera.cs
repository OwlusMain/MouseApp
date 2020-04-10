using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    float minFov = 0.5f;
    float maxFov = 60f;
    float sensitivity = 50f;
    public float moveMultiplier = 2f;

    bool camMove = false;
    Vector2 mouseCoords;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float curFov = Camera.main.fieldOfView;
        curFov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        curFov = Math.Max(curFov, minFov);
        curFov = Math.Min(curFov, maxFov);

        Camera.main.fieldOfView = curFov;

        moveMultiplier = 60 / curFov;

        if (Input.GetMouseButtonDown(2))
        {
            camMove = true;
            mouseCoords = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            camMove = false;
        }
        if (camMove)
        {
            Vector2 moveCoords = new Vector2(Input.mousePosition.x - mouseCoords.x, Input.mousePosition.y - mouseCoords.y);
            this.transform.position = new Vector3(this.transform.position.x - moveCoords.x / moveMultiplier, this.transform.position.y, this.transform.position.z - moveCoords.y / moveMultiplier);
            mouseCoords = Input.mousePosition;
        }
    }
}
