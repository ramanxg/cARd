using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public static TrashCan instance;

    private List<Sprite> trashCards;

    private void Awake()
    {
        instance = this;
        trashCards = new List<Sprite>();
    }


    public void addToTrash(Sprite card)
    {
        trashCards.Add(card);
        for (int i = 0; i < trashCards.Count; i++)
        {
            Debug.Log(trashCards[i]);
        }
    }

    public void emptyTrash()
    {

    }

    public void removeFromTrash()
    {

    }


}
