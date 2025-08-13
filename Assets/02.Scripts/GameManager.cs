using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DeckManager deckManager;
    public List<Card> playerHand = new List<Card>();
    public List<Card> aiHand = new List<Card>();

    public int playerScore;
    public int aiScore;

    public GameObject hitButton; // UI 버튼을 할당할 변수
    public GameObject standButton; // UI 버튼을 할당할 변수

    void Start()
    {
        StartNewGame();
        UpdateScores();
    }

    public void StartNewGame()
    {
        // 게임 초기화
        playerHand.Clear();
        aiHand.Clear();
        playerScore = 0;
        aiScore = 0;

        // 버튼 활성화
        hitButton.SetActive(true);
        standButton.SetActive(true);

        // 초기 카드 2장씩 분배
        playerHand.Add(deckManager.DealCard());
        aiHand.Add(deckManager.DealCard());
        playerHand.Add(deckManager.DealCard());
        aiHand.Add(deckManager.DealCard());
    }

    private void UpdateScores()
    {
        playerScore = GetScore(playerHand);
        aiScore = GetScore(aiHand);
    }

    private int GetScore(List<Card> hand)
    {
        int score = 0;
        int aceCount = 0;

        foreach (var card in hand)
        {
            if (card.value == CardValue.Ace)
            {
                aceCount++;
                score += 11;    // 에이스는 1 혹은 11이므로 우선 11로 계산
            }
            else
            {
                score += card.score;
            }
        }

        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }
        return score;
    }

    public void OnHitButtonClicked()
    {
        playerHand.Add(deckManager.DealCard());
        UpdateScores();

        Debug.Log($"플레이어 Hit! 카드: {GetHandString(playerHand)} (점수: {playerScore})");

        if (playerScore > 21)
        {
            Debug.Log("플레이어 버스트! 딜러 승리!");
            EndGame();
        }
    }

    // Stand 버튼 클릭 시 호출될 메서드
    public void OnStandButtonClicked()
    {
        Debug.Log("플레이어 Stand! 딜러 턴 시작.");
        hitButton.SetActive(false);
        standButton.SetActive(false);
        StartDealerTurn();
    }

    private void StartDealerTurn()
    {
        // 딜러의 카드 로그
        Debug.Log($"딜러의 모든 카드: {GetHandString(aiHand)} (점수: {aiScore})");

        while (aiScore < 17)
        {
            aiHand.Add(deckManager.DealCard());
            UpdateScores();
            Debug.Log($"딜러 Hit! 카드: {GetHandString(aiHand)} (점수: {aiScore})");
        }

        CheckWinner();
    }

    private void CheckWinner()
    {
        if (aiScore > 21)
        {
            Debug.Log("딜러 버스트! 플레이어 승리!");
        }
        else if (playerScore > aiScore)
        {
            Debug.Log("플레이어 승리!");
        }
        else if (aiScore > playerScore)
        {
            Debug.Log("딜러 승리!");
        }
        else
        {
            Debug.Log("무승부(Push)!");
        }
        EndGame();
    }

    private void EndGame()
    {
        Debug.Log("게임 종료!");
        // UI를 초기 상태로 되돌리거나 '다시 시작' 버튼을 활성화하는 로직
    }

    private string GetHandString(List<Card> hand)
    {
        return string.Join(", ", hand.Select(c => $"[{c.value}]"));
    }
    #region
    //private void Awake()
    //{
    //    InitPlayerHand();
    //    InitAiHand();
    //}

    //void Start()
    //{
    //    StartGame();
    //}

    //public void InitPlayerHand()
    //{
    //    Transform[] playerHandPos = GameObject.Find("Players").GetComponentsInChildren<Transform>();
    //    foreach (Transform playerHand in playerHandPos)
    //    {
    //        if (playerHand != this.transform && playerHand.name.StartsWith("Player1HandPos"))
    //            this.playerHand.Add(playerHand);
    //    }
    //}

    //public void InitAiHand()
    //{
    //    Transform[] aiHandPos = GameObject.Find("Players").GetComponentsInChildren<Transform>();
    //    foreach (Transform aiHand in aiHandPos)
    //    {
    //        if (aiHand != this.transform && aiHand.name.StartsWith("AIHandPos"))
    //            this.AIHand.Add(aiHand);
    //    }
    //}

    //public void StartGame()
    //{
    //    deckManager.ShuffleCard();
    //}
    #endregion
}
