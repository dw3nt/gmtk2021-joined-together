using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    void FixedUpdate()
    {
        Vector3 targetPos = target.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
    }
}
