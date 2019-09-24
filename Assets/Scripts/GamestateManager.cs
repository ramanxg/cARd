using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GoogleARCore;

// Maintains the state of the deck.
public class GamestateManager : MonoBehaviour
{
    public static GamestateManager instance;

    // contains all 52 card sprites
    public Sprite[] cardsSprites;
    // virtual deck containing all the sprites in the game
    public List<Sprite> deck;

    // prefab used to instantiate AR object
    public GameObject cardTemplate;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        deck = new List<Sprite>();
        InitializeDeck();
        ShuffleDeck();
    }

    // sets deck to have 52 cards
    public void InitializeDeck()
    {
        deck.Clear();
        foreach (Sprite s in cardsSprites)
        {
            deck.Add(s);
        }
    }

    // shuffles the deck
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Sprite temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // removes the first card from the deck and returns the sprite
    public Sprite DrawCard()
    {
        if (deck.Count > 0)
        {
            Sprite s = deck[0];
            deck.RemoveAt(0);
            return s;
        }
        return null;
    }

    // instantiates an AR card object with a given sprite and returns the gameobject
    public GameObject InstantiateCard(Sprite s)
    {
        // finds the plane to instantiate on
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 pos = Input.mousePosition;
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        Vector2 pos = Input.GetTouch(0).position;
#endif

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        // if casting onto a plane
        if (Frame.Raycast(pos.x, pos.y, raycastFilter, out hit))
        {
            Debug.Log("Found plane");
            // creates template
            GameObject copy = Instantiate(cardTemplate);
            // gives it a card material
            Renderer rend = copy.transform.Find("Top").GetComponent<Renderer>();
            var tempMaterial = new Material(rend.sharedMaterial);
            tempMaterial.shader = Shader.Find("ARCore/DiffuseWithLightEstimation");
            tempMaterial.SetTexture("_MainTex", s.texture);
            rend.sharedMaterial = tempMaterial;
            // changes name
            copy.name = s.name;
            Card c = copy.GetComponent<Card>();
            // sets card on the plane
            c.SetSelectedPlane(hit);
            c.mySprite = s;
            copy.transform.localScale = new Vector3(0.1f, 0.0001f, 0.15f);
            return copy;
        }
        return null;
    }
}
