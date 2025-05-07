using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Other : MonoBehaviour
{
    public Transform currentCamera;
    void Start()
    {
        currentCamera.position = new Vector3(-1, 10, -14);
        currentCamera.eulerAngles = new Vector3(15, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
