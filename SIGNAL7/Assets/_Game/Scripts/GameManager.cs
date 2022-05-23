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

        SpawnSignals();

        StartCoroutine(Countdown());
    }

    [SerializeField] TextMeshProUGUI gameResultText;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] Transform[] spawnLocations;
    [SerializeField] Signal[] signals;

    private int signalCount = 4;

    private bool gameOver = false;
    private bool gameStarted = false;

    public bool IsGameRunning()
    {
        return gameStarted && !gameOver;
    }

    private IEnumerator Countdown()
    {
        gameResultText.gameObject.SetActive(true);

        for(int i = 3; i > 0; i--)
        {
            gameResultText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        gameStarted = true;

        gameResultText.text = "GO!";

        yield return new WaitForSeconds(0.5f);

        gameResultText.gameObject.SetActive(false);
    }

    private void SpawnSignals()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        foreach(Transform t in spawnLocations)
        {
            spawnPositions.Add(t.position);
        }

        int randomIndex;

        foreach(Signal s in signals)
        {
            randomIndex = Random.Range(0, spawnPositions.Count);
            s.transform.position = spawnPositions[randomIndex];
            spawnPositions.RemoveAt(randomIndex);
        }
    }

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
            else
            {
                gameResultText.text = "you win!";
                scoreManager.AddWinBonus();
            }

            if (scoreManager.IsItAHighScore())
            {
                scoreManager.EnterHighScorePrompt();
            }

            gameResultText.gameObject.SetActive(true);
            gameOver = true;
            Time.timeScale = 0.5f;
        }
    }
}
