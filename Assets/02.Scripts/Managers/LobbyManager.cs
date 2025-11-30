using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics.Tracing;

public class LobbyManager : MonoBehaviour
{
    public Button gameStart;
    public Button gameRule;
    public Button exit;
    public Button close;
    public Button nextButton;
    public GameObject rule;
    public Text page01;
    public Text page02;

    void Start()
    {
        rule.SetActive(false);

        gameStart.onClick.AddListener(OnStart);
        gameRule.onClick.AddListener(OnRule);
        exit.onClick.AddListener(OnExit);
        close.onClick.AddListener(OnClose);
        nextButton.onClick.AddListener(OnNextPage);
    }

    public void OnStart()
    {
        SceneManager.LoadScene("01.BlackJackScene");
    }

    public void OnRule()
    {
        rule.SetActive(true);
        nextButton.gameObject.SetActive(true);
        page02.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        page01.gameObject.SetActive(true);
        page02.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        rule.SetActive(false);
    }

    public void OnNextPage()
    {
        page01.gameObject.SetActive(false);
        page02.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(false);
    }

    public void OnExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        Debug.Log("게임종료");
#endif
    }
}
