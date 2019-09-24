using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

// Allows movement for the AR card objects on the field.
public class MovementManager : MonoBehaviour
{
    // the AR card object being dragged around
    private GameObject focusObj = null;
    // the Card component
    private Card focusObjCard = null;

    public RectTransform trashCan;
    

    void Update()
    {
#if UNITY_EDITOR
        RaycastMove();
#elif UNITY_ANDROID
        RaycastMoveAndroid();
#endif
    }

    // moving AR cards on the PC (uses mouse)
    void RaycastMove()
    {
        // finding object to move
        if (focusObj == null && Input.GetButtonDown("Fire1") && Input.mousePosition.y > Hand.CARDSIZE.y + Hand.CARDMARGIN.y)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // only raycast onto cards
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
            {
                OnFind(hit);
            }
        }

        // if moving
        if (focusObj)
        {
            // dereferences the focusobj
            if (Input.GetButtonUp("Fire1"))
            {
                OnRelease(Input.mousePosition);
            }
            // moves the focusobj around on the field
            else if (Input.GetButton("Fire1"))
            {
                Vector2 pos = Input.mousePosition;
                TrackableHit hit;
                TrackableHitFlags raycastFilter =
                    TrackableHitFlags.PlaneWithinBounds |
                    TrackableHitFlags.PlaneWithinPolygon;

                if (Frame.Raycast(pos.x, pos.y, raycastFilter, out hit))
                {
                    OnMove(hit);
                }
            }
        }
    }

    // moving AR cards on android (uses screen touches)
    void RaycastMoveAndroid()
    {
        // finding object to move
        if (focusObj == null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && 
            Input.GetTouch(0).position.y > Hand.CARDSIZE.y + Hand.CARDMARGIN.y)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            // only raycast onto cards
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
            {
                OnFind(hit);
            }
        }

        // if moving
        if (focusObj && Input.touchCount > 0)
        {
            // moves the focusobj around on the field
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 pos = Input.GetTouch(0).position;
                TrackableHit hit;
                TrackableHitFlags raycastFilter =
                    TrackableHitFlags.PlaneWithinBounds |
                    TrackableHitFlags.PlaneWithinPolygon;

                if (Frame.Raycast(pos.x, pos.y, raycastFilter, out hit))
                {
                    OnMove(hit);
                }
            }
            // dereferences the focusobj
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnRelease(Input.GetTouch(0).position);
            }
        }
    }

    // touch AR card
    public void OnFind(RaycastHit hit)
    {
        focusObj = hit.transform.gameObject;
        focusObjCard = focusObj.GetComponent<Card>();
    }

    // move AR card
    public void OnMove(TrackableHit hit)
    {
        focusObjCard.AttachToAnchor(hit);
    }

    // release AR card
    public void OnRelease(Vector2 releasePosition)
    {
        // if release onto hand, add it to the hand
        if(releasePosition.y < Hand.CARDSIZE.y + Hand.CARDMARGIN.y)
        {
            Sprite mySprite = focusObjCard.mySprite;
            // adds the card into the hand
            Image card = Hand.instance.AddCard(mySprite);
            // inserts the card in the hand based on release position
            Hand.instance.InsertCard(card, releasePosition);
            // destroys the AR card
            Destroy(focusObj);
        }
        
        Debug.Log("RELEASE POSITION: " + releasePosition);
        Debug.Log("WIDTH: " + trashCan.rect.width);
        Debug.Log("HEIGHT: " + trashCan.rect.height);
        if (releasePosition.y > Hand.CANVASSIZE.height - trashCan.rect.height && releasePosition.x < trashCan.rect.width)
        {
            TrashCan.instance.addToTrash(focusObjCard.mySprite);
            Destroy(focusObjCard.gameObject);
        }
        // no more focus object
        focusObj = null;
        focusObjCard = null;
    }
}
