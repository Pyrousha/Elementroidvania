using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private static readonly float camHeight = 22.5f;
    private static readonly float camWidth = 40.0f;

    public float Top { get; private set; }
    public float Bottom { get; private set; }
    public float Left { get; private set; }
    public float Right { get; private set; }

    void Awake()
    {
        BoxCollider2D cameraBounds = GetComponent<BoxCollider2D>();

        Top = cameraBounds.bounds.max.y;// - camHeight / 2.0f;
        Bottom = cameraBounds.bounds.min.y;// + camHeight / 2.0f;
        Debug.Log($"Top: {Top}, Bottom: {Bottom}");
        Top -= camHeight / 2.0f;
        Bottom += camHeight / 2.0f;
        Debug.Log($"TopAfter: {Top}, BottomAfter: {Bottom}");
        Left = cameraBounds.bounds.min.x + camWidth / 2.0f;
        Right = cameraBounds.bounds.max.x - camWidth / 2.0f;
    }
}
