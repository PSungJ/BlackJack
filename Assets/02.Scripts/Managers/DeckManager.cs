using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Rank
    {
        Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }
    public Rank rank;           // 카드 숫자 (1~11, J/Q/K는 10)
    public int value;           // 실제 게임 점수용 값 (A=1, J/Q/K=10)
    public Sprite frontSprite;  // 카드 앞면
    public Sprite backSprite;   // 카드 뒷면 (공용)

    public override string ToString()
    {
        return $"{rank} ({value})";
    }
}
    public class DeckManager : MonoBehaviour
{
    [Header("카드 앞면 52장 Sprite")]
    public Sprite[] frontSprites;   // Inspector에서 52장 넣기

    [Header("공용 카드 뒷면 Sprite")]
    public Sprite backSprite;

    private Queue<Card> deck = new Queue<Card>();

    /// 덱 초기화 및 섞기
    public void InitializeDeck()
    {
        deck.Clear();
        List<Card> allCards = new List<Card>();

        for (int v = 1; v <= 13; v++) // A~K
        {
            for (int s = 0; s < 4; s++) // 문양 4개, 하지만 suit 제거됨
            {
                int spriteIndex = s * 13 + (v - 1);

                Card.Rank rank = (Card.Rank)v;
                int value = v > 10 ? 10 : v; // J/Q/K = 10, A = 1

                allCards.Add(new Card
                {
                    rank = rank,
                    value = value,
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