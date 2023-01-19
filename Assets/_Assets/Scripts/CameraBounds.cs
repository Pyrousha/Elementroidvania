using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private const float camHeight = 16.875f;
    private const float camWidth = 30.0f;

    private float top;
    private float bottom;
    private float left;
    private float right;

    void Awake()
    {
        BoxCollider2D cameraBounds = GetComponent<BoxCollider2D>();

        top = cameraBounds.bounds.max.y - camHeight / 2.0f;
        bottom = cameraBounds.bounds.min.y + camHeight / 2.0f;
        left = cameraBounds.bounds.min.x + camWidth / 2.0f;
        right = cameraBounds.bounds.max.x - camWidth / 2.0f;

        Debug.Log($"top: {top}, bottom: {bottom}, left: {left}, right {right}");
    }

    /// <summary>
    /// Given a position as a vector3, returns that position clamped within this bounds.
    /// </summary>
    /// <param name="_unboundedPosition"></param>
    /// <returns></returns>
    public Vector3 ClampPosition(Vector3 _unboundedPosition)
    {
        //Use current bounds (room) to keep camera from going through walls
        float targX = Mathf.Clamp(_unboundedPosition.x, left, right);
        float targY = Mathf.Clamp(_unboundedPosition.y, bottom, top);
        return new Vector3(targX, targY, _unboundedPosition.z);
    }
}
