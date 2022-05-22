using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //Time.timeScale = 0.5f;
    }

    [SerializeField] TextMeshProUGUI gameResultText;
    [SerializeField] ScoreManager scoreManager;

    private int signalCount = 4;
    public bool gameOver = false;
    public void EliminateSignal(bool playerWasEliminated, bool eliminatedByPlayerTrail, Color signalColor)
    {
        Debug.Log("Signal eliminated");

        if(eliminatedByPlayerTrail)
        {
            scoreManager.ShowAIEliminationBonus(signalColor);
        }

        signalCount--;

        if(signalCount == 1 || playerWasEliminated)
        {
            if (playerWasEliminated)
            {
                gameResultText.text = "you lose!";
            }

            gameResultText.gameObject.SetActive(true);
            gameOver = true;
            Time.timeScale = 0.5f;
        }
    }
}
