using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    #region
    //public List<GameObject>cardDeck = new List<GameObject>();
    //public List<GameObject> discardPile = new List<GameObject>();

    //void Awake()
    //{
    //    GameObject mainDeck = GameObject.Find("BlueBackCards");
    //    if (mainDeck != null)
    //    {
    //        Transform[] cards = mainDeck.GetComponentsInChildren<Transform>();
    //        foreach (Transform card in cards)
    //        {
    //            if (card.gameObject != mainDeck)
    //                cardDeck.Add(card.gameObject);
    //        }
    //    }
    //}

    //public void ShuffleCard()
    //{
    //    // 리스트의 마지막 요소부터 첫 번째 요소까지 반복
    //    for (int i = cardDeck.Count - 1; i > 0; i--)
    //    {
    //        // 현재 요소(i)와 0부터 i까지의 무작위 인덱스(j)를 선택
    //        int j = Random.Range(0, i + 1);

    //        // 현재 요소와 무작위로 선택된 요소의 위치를 바꿉니다.
    //        GameObject temp = cardDeck[i];
    //        cardDeck[i] = cardDeck[j];
    //        cardDeck[j] = temp;
    //    }
    //}

    //public GameObject DealCard(GameObject dealtCard)
    //{
    //    if (cardDeck.Count == 0)
    //        return null;

    //    dealtCard = cardDeck[0];           // 덱의 맨 위 카드
    //    cardDeck.RemoveAt(0);              // 덱에서 제거

    //    Debug.Log($"Card : {dealtCard.name}, 남은 카드 : {cardDeck.Count}");
    //    return dealtCard;
    //}
    #endregion
    public List<Card> deck;

    void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
    }
    private void InitializeDeck()
    {
        deck = new List<Card>();
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardValue value in System.Enum.GetValues(typeof(CardValue)))
            {
                deck.Add(new Card(suit, value));
            }
        }
    }

    private void ShuffleDeck()
    {
        // Fisher-Yates 셔플 알고리즘
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Card temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }
    }

    public Card DealCard()
    {
        if (deck.Count == 0)
        {
            InitializeDeck();
            ShuffleDeck();
            Debug.Log("덱이 소진되어 다시 섞었습니다.");
        }

        Card dealtCard = deck[0];
        deck.RemoveAt(0);
        return dealtCard;
    }
}
