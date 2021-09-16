using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameProgressManager : MonoBehaviour
{
    public List<GameObject> player1Cards;
    public List<GameObject> player2Cards;

    public Button btnNewGame;
    public Button[] btnPlayerGo;
    public Button[] btnPlayerEnd;

    public Text resultText;

    private Random rnd = new Random();
    
    enum PlayerInputState
    {
        Blocking,
        WaitingInput,
        Go,
        End,
    }

    private PlayerInputState[] playerState = new PlayerInputState[2];
    private int[] playerCardIndex = new int[2];
    private int[] playerTotal = new int[2];

    
    void Awake()
    {
        btnNewGame.onClick.AddListener(NewGame);
    }

    void Start()
    {
        NewGame();
    }
    
    private void NewGame()
    {
        
        //StartCoroutine(GameLoop()); //协程版本
        GameLoopVer2().Forget(); //unitask版本
    }
    
    void ClearGameState()
    {
        playerState[0] = PlayerInputState.Blocking;
        playerState[1] = PlayerInputState.Blocking;
        player1Cards.ForEach(p => p.gameObject.SetActive(false));
        player2Cards.ForEach(p => p.gameObject.SetActive(false));
        for (int i = 0; i < playerCardIndex.Length; ++i)
        {
            playerCardIndex[i] = 0;
            playerTotal[i] = 0;
        }
        resultText.text = "";
    }

    public void PlayerGo(int player)
    {
        int index = playerCardIndex[player]++;
        int newCard = rnd.Next(1, 10);
        List<GameObject> cards = (player == 0) ? player1Cards : player2Cards;
        var cardObj = cards[index];
        cardObj.gameObject.SetActive(true);
        cardObj.GetComponentInChildren<Text>().text = newCard.ToString();
        playerTotal[player] += newCard;

        if (playerTotal[player] <= 21)
        {
            playerState[player] = PlayerInputState.Go;    
        }
        else
        {
            playerState[player] = PlayerInputState.End;
        }
    }

    public void PlayerEnd(int player)
    {
        playerState[player] = PlayerInputState.End;
    }
    


    void ShowResult()
    {
        bool player1Bomb = false;
        bool player2Bomb = false;
        if (playerTotal[0] > 21) player1Bomb = true;
        if (playerTotal[1] > 21) player2Bomb = true;
        
        string rst = $"玩家1：{playerTotal[0]} VS 玩家2：{playerTotal[1]}\n";
        if (player1Bomb && player2Bomb)
        {
            rst += "平局";
        }
        else if (player1Bomb && !player2Bomb)
        {
            rst += "玩家2胜利";
        }else if (!player1Bomb && player2Bomb)
        {
            rst += "玩家1胜利";
        }
        else
        {
            if (playerTotal[0] == playerTotal[1])
            {
                rst += "平局";
            }else if (playerTotal[0] > playerTotal[1])
            {
                rst += "玩家1胜利";
            }
            else
            {
                rst += "玩家2胜利";
            }
        }

        resultText.text = rst;
    }


    //协程版本
    IEnumerator GameLoop()
    {
        ClearGameState();
        while (true)
        {
            yield return PlayerInput(0);
            yield return PlayerInput(1);
            if (playerState[0] == PlayerInputState.End && playerState[1] == PlayerInputState.End)
            {
                ShowResult();
                yield break;
            }
        }
    }

    //协程版本
    private IEnumerator PlayerInput(int player)
    {
        if (playerState[player] == PlayerInputState.End)
            yield break;
        
        int otherPlayer = (player == 0) ? 1 : 0;

        btnPlayerGo[player].gameObject.SetActive(true);
        btnPlayerEnd[player].gameObject.SetActive(true);
        btnPlayerGo[otherPlayer].gameObject.SetActive(false);
        btnPlayerEnd[otherPlayer].gameObject.SetActive(false);
        
        playerState[player] = PlayerInputState.WaitingInput;
        yield return new WaitUntil(() => playerState[player] != PlayerInputState.WaitingInput); //等待输入
    }
    
    
    //unitask版本
    async UniTask GameLoopVer2()
    {
        ClearGameState();
        while (true)
        {
            await PlayerInputVer2(0);
            await PlayerInputVer2(1);
            if (playerState[0] == PlayerInputState.End && playerState[1] == PlayerInputState.End)
            {
                ShowResult();
                return;
            }
        }
    }

    
    //unitask版本
    async UniTask PlayerInputVer2(int player)
    {
        if (playerState[player] == PlayerInputState.End)
            return;
        
        int otherPlayer = (player == 0) ? 1 : 0;

        btnPlayerGo[player].gameObject.SetActive(true);
        btnPlayerEnd[player].gameObject.SetActive(true);
        btnPlayerGo[otherPlayer].gameObject.SetActive(false);
        btnPlayerEnd[otherPlayer].gameObject.SetActive(false);
        
        playerState[player] = PlayerInputState.WaitingInput;
        await UniTask.WaitUntil(() => playerState[player] != PlayerInputState.WaitingInput);
    }
}
