using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Card
{
    public string suit;     // 카드 문양 (♠, ♥, ?, ♣)
    public int value;       // 카드 숫자 (1~11, J/Q/K는 10)
    public Sprite frontSprite; // 카드 앞면
    public Sprite backSprite;  // 카드 뒷면 (공용)
}
    public class DeckManager : MonoBehaviour
{
    [Header("카드 앞면 52장 Sprite")]
    public Sprite[] frontSprites;   // Inspector에서 52장 넣기

    [Header("공용 카드 뒷면 Sprite")]
    public Sprite backSprite;

    private Queue<Card> deck = new Queue<Card>();

    private readonly string[] suits = { "♠", "♥", "♦", "♣" };

    /// 덱 초기화 및 섞기
    public void InitializeDeck()
    {
        deck.Clear();
        List<Card> allCards = new List<Card>();

        for (int s = 0; s < suits.Length; s++)
        {
            for (int v = 1; v <= 13; v++)
            {
                int spriteIndex = s * 13 + (v - 1);
                allCards.Add(new Card
                {
                    suit = suits[s],
                    value = v > 10 ? 10 : v, // J/Q/K는 10 처리
                    frontSprite = frontSprites[spriteIndex],
                    backSprite = backSprite
                });
            }
        }

        // LINQ를 이용해 무작위로 섞음
        allCards = allCards.OrderBy(c => Random.value).ToList();
        foreach (var card in allCards)
            deck.Enqueue(card);
    }

    /// 카드 1장 뽑기
    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("덱이 비었습니다. 새로 초기화합니다.");
            InitializeDeck();
        }

        return deck.Dequeue();
    }
}