using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CamBoundsTrigger : MonoBehaviour
{
    private Transform cameraTransform;
    private CameraBounds currCamBounds;

    private float zOffset = -10f;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targCamPos = transform.position + new Vector3(0, 0, zOffset); //Default camera position to center on player
        if (currCamBounds != null)
        {
            //Use current bounds (room) to keep camera from going through walls
            float targX = Mathf.Clamp(transform.position.x, currCamBounds.Left, currCamBounds.Right);
            float targY = Mathf.Clamp(transform.position.y, currCamBounds.Bottom, currCamBounds.Top);
            targCamPos = new Vector3(targX, targY, zOffset);
        }

        Debug.Log(targCamPos);

        cameraTransform.position = targCamPos;
    }

    /// <summary>
    /// Called when the player touches a new camera bounds hitbox, changes the reference to the current camera bounds (room).
    /// </summary>
    /// <param name="_collider2D"></param>
    void OnTriggerEnter2D(Collider2D _collider2D)
    {
        currCamBounds = _collider2D.GetComponent<CameraBounds>();
    }
}
