using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotate : MonoBehaviour
{

    [SerializeField] private float rotateDirection = 100f;

    
    private void Update()
    {
        //rotate the platform at the speed of the set rotateDirection
        transform.localRotation = Quaternion.Euler(0, Time.time * rotateDirection, 0);
    }
}
