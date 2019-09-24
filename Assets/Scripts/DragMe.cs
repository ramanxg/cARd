using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attached onto every card in the hand. Enables them to be dragged around.
[RequireComponent(typeof(Image))]
public class DragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;

    // icon object that is created when dragging on canvas
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    // image component on this card
    Image myImage;

    private void Start()
    {
        myImage = GetComponent<Image>();
    }

    // Unity calls when beginning to drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        // gets canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        m_DraggingIcon = new GameObject("icon");
        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        // adds an image on icon
        Image image = m_DraggingIcon.AddComponent<Image>();
        image.sprite = GetComponent<Image>().sprite;
        image.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;
        SetDraggedPosition(eventData);

        // disables hand image
        myImage.enabled = false;
    }

    // Unity calls when dragging
    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    // process dragging data
    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        // drags around icon on canvas
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    // Unity calls when release drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_DraggingIcon != null)
        {
            // destroys the icon
            Destroy(m_DraggingIcon);
            RectTransform trashCan = FindObjectOfType<MovementManager>().trashCan;
            if (eventData.position.y > Hand.CANVASSIZE.height - trashCan.rect.height && eventData.position.x < trashCan.rect.width)
            {
                TrashCan.instance.addToTrash(myImage.sprite);
                Hand.instance.RemoveCard(myImage);
            }

            // if above threshold, remove card from hand and instantiate AR card onto the field
            else if (eventData.position.y > Hand.CARDSIZE.y + Hand.CARDMARGIN.y)
            {                
                Image targetImage = myImage;
                GameObject instantiatedCard = GamestateManager.instance.InstantiateCard(targetImage.sprite);
                // if successfully instantiated a card
                if(instantiatedCard != null)
                {
                    Hand.instance.fieldCards.Add(instantiatedCard);
                    Hand.instance.RemoveCard(targetImage);
                }
                else
                {
                    Hand.instance.InsertCard(eventData.pointerDrag.GetComponent<Image>(), eventData.position);
                }
            }
            // else re-enable card hand
            else
            {
                Hand.instance.InsertCard(eventData.pointerDrag.GetComponent<Image>(), eventData.position);
            }
        }
    }
}