using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotate : MonoBehaviour
{

    [SerializeField] float rotateDirection = 100f;

    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, Time.time * rotateDirection, 0);
    }
}
