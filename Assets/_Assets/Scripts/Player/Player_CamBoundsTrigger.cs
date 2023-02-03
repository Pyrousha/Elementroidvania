using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CamBoundsTrigger : MonoBehaviour
{
    private Transform cameraTransform;
    private CameraBounds currCamBounds;

    private float zOffset = -10f;

    private Coroutine currLerp = null;
    private bool isCurrLerpNull = true;
    private float lerpSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if ((currLerp != null) || (!isCurrLerpNull))
        {
            Debug.Log("currently lerping, do not move camera");
            return; //Don't move camera if lerping
        }

        if (currCamBounds != null)
            cameraTransform.position = currCamBounds.ClampPosition(new Vector3(transform.position.x, transform.position.y, zOffset));
        else
            cameraTransform.position = new Vector3(transform.position.x, transform.position.y, zOffset); //Default camera position to center on player
    }

    /// <summary>
    /// Called when the player touches a new camera bounds hitbox, changes the reference to the current camera bounds (room).
    /// </summary>
    /// <param name="_collider2D"></param>
    void OnTriggerEnter2D(Collider2D _collider2D)
    {
        CameraBounds newCamBounds = _collider2D.GetComponent<CameraBounds>();
        if (newCamBounds == currCamBounds)
            return;

        currCamBounds = newCamBounds;

        //Lerp to new bounds
        if (currLerp != null)
        {
            //If already lerping, stop that lerp and start this one
            StopCoroutine(currLerp);
        }

        Vector3 endLerpCamPos = currCamBounds.ClampPosition(new Vector3(transform.position.x, transform.position.y, zOffset));
        currLerp = StartCoroutine(LerpToNewScreen(endLerpCamPos));
        isCurrLerpNull = false;
    }

    IEnumerator LerpToNewScreen(Vector3 _targPos)
    {
        yield return null;

        while (true)
        {
            float currDist = Vector3.Distance(cameraTransform.position, _targPos);
            //Debug.Log("currdist: " + currDist + $" currPos: {cameraTransform.position}, targPos: {_targPos}");
            if (currDist > 0.1f)
            {
                //Not close enough, keep lerping
                Vector3 newPos = Vector3.Lerp(cameraTransform.position, _targPos, lerpSpeed / currDist);
                newPos.z = zOffset;
                cameraTransform.position = newPos;
            }
            else
                break;

            yield return null;
        }

        //Close enough, end lerp
        cameraTransform.position = _targPos;
        //Debug.Log("Set curr lerp to null");
        currLerp = null;
        isCurrLerpNull = true;
    }
}
