using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardSuit { Spades, Hearts, Diamonds, Clubs }
public enum CardValue { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

// 인스펙터에서 볼 수 있게 만듦
[System.Serializable]
public class Card
{
    public CardSuit suit;
    public CardValue value;
    public int score; // 게임에서 사용될 점수

    public Card(CardSuit suit, CardValue value)
    {
        this.suit = suit;
        this.value = value;
        // 카드의 점수를 초기화
        this.score = GetScore(value);
    }

    private int GetScore(CardValue value)
    {
        if (value >= CardValue.Ten) return 10;
        return (int)value;
    }
}
