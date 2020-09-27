using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothTime = 0.3F;
    public float followSpeedModifier = 20f;
    private Vector3 velocity = Vector3.zero;

    void Start(){
        transform.position = GetTargetPosition();
    }

    void Update()
    {
        var newPosition = Vector3.SmoothDamp(transform.position, GetTargetPosition(), ref velocity, smoothTime);
        // var newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeedModifier);

        transform.position = newPosition;
    }

    public Vector3 GetTargetPosition(){
        return target.position + offset;
    }
}
