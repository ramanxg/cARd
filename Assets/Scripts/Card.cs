using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

// Attached onto every AR card object. Enforces uniform layout.
public class Card : MonoBehaviour
{
    // the anchor this AR card is attached to
    private Anchor anchor;
    // the plane this AR card is placed on
    private DetectedPlane detectedPlane;
    // sprite on this card
    public Sprite mySprite;

    // Update is called once per frame
    void Update()
    {
        if(detectedPlane == null)
        {
            return;
        }
        // maintains same x rotation as plane
        if (transform.eulerAngles.x != detectedPlane.CenterPose.rotation.eulerAngles.x)
        {
            Vector3 planeRotation = detectedPlane.CenterPose.rotation.eulerAngles;
            //planeRotation.y = 0;
            //planeRotation.z = 0;
            transform.eulerAngles = planeRotation;
        }

        //// maintains same height on playing field
        //if (transform.position.y != detectedPlane.CenterPose.position.y)
        //{
        //    transform.position = new Vector3(transform.position.x, detectedPlane.CenterPose.position.y, transform.position.z);
        //}
    }

    public void SetSelectedPlane(TrackableHit hit)
    {
        Debug.Log("Setting plane to: " + hit.Trackable);
        detectedPlane = (DetectedPlane)hit.Trackable;
        AttachToAnchor(hit);
    }

    public void AttachToAnchor(TrackableHit hit)
    {
        // if moving off plane
        if(hit.Trackable == null)
        {
            Debug.Log("Card off plane!");
            return;
        }

        // move to hit position
        transform.position = hit.Pose.position;

        // if moving to new plane
        if (hit.Trackable != detectedPlane)
        {
            Debug.Log("Moving Card to new plane!");
            detectedPlane = (DetectedPlane)hit.Trackable;
            return;
        }

        // if does not have an anchor or too far away from current anchor
        // give it an anchor
        if (anchor == null || (anchor.transform.position - transform.position).magnitude > 1)
        {
            // gets all existing anchors on this plane
            List<Anchor> existingAnchors = new List<Anchor>();
            detectedPlane.GetAllAnchors(existingAnchors);

            bool anchorFound = false;
            // look for a nearby existing anchor
            foreach (Anchor a in existingAnchors)
            {
                // if found a nearby anchor
                if ((a.transform.position - hit.Pose.position).magnitude < 1)
                {
                    Debug.Log("Moving card to existing anchor!");
                    anchor = a;
                    anchorFound = true;
                    break;
                }
            }

            //if did not find an anchor, create a new one
            if(!anchorFound)
            {
                Debug.Log("Creating new anchor!");
                anchor = detectedPlane.CreateAnchor(hit.Pose);
            }

            transform.SetParent(anchor.transform);
        }

        //Debug.Log("Anchor Position: " + anchor.transform.position);
        //Debug.Log("Distance From Anchor: " + transform.localPosition.magnitude);   
    }
}
