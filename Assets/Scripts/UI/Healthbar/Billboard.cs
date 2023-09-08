using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }
    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation * originalRotation;
    }
}
