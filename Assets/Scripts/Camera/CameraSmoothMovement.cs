using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт, для постоянного плавного движения камеры к объекту, за которым она следует.
/// </summary>
public class CameraSmoothMovement : MonoBehaviour
{
    [SerializeField] GameObject objectToFocusOn;

    // Works after all update functions called.
    private void LateUpdate()
    {
        // Target position of the camera.
        Vector3 positionToGo = objectToFocusOn.transform.position;
        // Smooth position of the camera.
        Vector3 smoothPosition = Vector3.Lerp(transform.position, positionToGo, 0.07F);
        transform.position = smoothPosition;
    }
}
