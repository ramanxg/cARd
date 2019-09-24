using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Maintains the state of the hand.
public class Hand : MonoBehaviour
{
    public static Hand instance;

    // contains all the AR card objects on the FIELD
    public List<GameObject> fieldCards;
    // contains all the images in the HAND
    public List<Image> handCards;
    // parents the hand card images under this object
    public GameObject hand;
    // canvas to query device dimensions
    Canvas canvas;

    // CONSTANTS
    public static Vector2 CARDSIZE = new Vector2(162.75f, 274);
    public static Rect CANVASSIZE;
    public static Vector2 CARDMARGIN = new Vector2(5, 20);

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fieldCards = new List<GameObject>();
        canvas = GetComponent<Canvas>();
        CANVASSIZE = canvas.GetComponent<RectTransform>().rect;
    }

    // draws a card from the deck and adds it to the hand
    // called by Canvas->Deck button
    public void DrawCard()
    {
        // max 7 cards
        if (handCards.Count < 7)
        {
            Sprite sprite = GamestateManager.instance.DrawCard();
            // if deck did not run out of cards
            if(sprite != null)
            {
                AddCard(sprite);
                SortHand();
            }
        }
    }

    // adds a card to the hand
    public Image AddCard(Sprite s)
    {
        // creates Image object
        GameObject handCard = new GameObject("handCard");
        handCard.transform.SetParent(hand.transform, false);
        handCard.transform.SetAsLastSibling();
        handCard.AddComponent<DragMe>();

        // gets image component
        Image image = handCard.GetComponent<Image>();
        // sets sprite on image
        image.sprite = s;
        image.GetComponent<RectTransform>().sizeDelta = CARDSIZE;
        // sets anchor
        RectTransform rt = image.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(.5f, 0);
        rt.anchorMax = new Vector2(.5f, 0);
        rt.pivot = new Vector2(.5f, 0);
        // adds card to hand
        handCards.Add(image);
        return image;
    }

    // remove a card from the hand
    public void RemoveCard(Image i)
    {
        handCards.Remove(i);
        SortHand();
    }

    // rearranges the cards in the hand based on the order it was drawn
    void SortHand()
    {
        int numCards = handCards.Count;
        // if no cards to sort, return
        if(numCards == 0)
        {
            return;
        }
        // offset of the far left card
        Vector2 initOffset = new Vector2(-1 * ((numCards - 1) / 2.0f) * (CARDSIZE.x + CARDMARGIN.x), CARDMARGIN.y);
        // loop through cards to set their positions
        for (int j = 0; j < numCards; j++)
        {
            Image card = handCards[j];
            card.GetComponent<RectTransform>().localPosition = initOffset + new Vector2(j * (CARDSIZE.x + CARDMARGIN.x), 0);
            card.enabled = true;
        }
    }

    // inserts a card based on its release position
    public void InsertCard(Image card, Vector2 releasePosition)
    {
        Vector2 originalPos = card.GetComponent<RectTransform>().position;
        // if releasing near the original position
        if ((releasePosition - originalPos).magnitude < CARDSIZE.x + CARDMARGIN.x)
        {
            // simply re-enable the image
            card.enabled = true;
            SortHand();
            return;
        }

        // if releasing on the edge
        Vector2 leftEdge = handCards[0].GetComponent<RectTransform>().position;
        Vector2 rightEdge = handCards[handCards.Count - 1].GetComponent<RectTransform>().position;
        // left edge
        if (releasePosition.x < leftEdge.x)
        {
            // removes card from hand
            handCards.Remove(card);
            // insert card at the beginning
            handCards.Insert(0, card);
            SortHand();
            return;
        }
        // right edge
        else if(releasePosition.x > rightEdge.x)
        {
            // removes card from hand
            handCards.Remove(card);
            // insert card at the end
            handCards.Insert(handCards.Count, card);
            SortHand();
            return;
        }

        // if releasing between two other cards
        // at this point, at least 3 cards
        int myIndex = handCards.IndexOf(card);
        // iterate through second to last card
        for(int j = 0; j < handCards.Count - 1; j++)
        {
            if(j == myIndex || j + 1 == myIndex)
            {
                continue;
            }
            Vector2 leftCard = handCards[j].GetComponent<RectTransform>().position;
            Vector2 rightCard = handCards[j + 1].GetComponent<RectTransform>().position;
            // if between two other cards, insert in the middle
            if(releasePosition.x > leftCard.x && releasePosition.x < rightCard.x)
            {
                handCards.Remove(card);
                // if origin index is less than target index
                if (myIndex < j + 1)
                {
                    handCards.Insert(j, card);
                }
                // if origin index is greater than target index
                else
                {
                    handCards.Insert(j + 1, card);
                }
                SortHand();
                return;
            }
        }

        // default setback
        card.enabled = true;
        SortHand();
    }
}
