using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI score;
    [SerializeField]
    TextMeshProUGUI AIEliminationBonusText;
    [SerializeField]
    TextMeshProUGUI highScoreTitle;
    [SerializeField]
    TextMeshProUGUI highScoreBody;

    int currentScore = 0;
    int scoreIncrement = 1;
    int AIEliminationBonus = 100;
    int winBonus = 500;

    float timeElapsed = 0f;
    float addToScoreInterval = 0.25f;

    int numHighScores = 5;

    bool blinking = false;
    bool axisInput = false;
    bool enteringHighScore = false;
    int highScoreRankingIndex = -1;
    int highScorePromptIndex = 0;
    int currentPromptLetterIndex = 0;
    string currentHighScoreName = "AAA";
    string blinkHighScoreName = "_AA";
    private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    string[] currentHighScoresRaw;

    private void Awake()
    {
        currentHighScoresRaw = new string[numHighScores];

        AIEliminationBonusText.gameObject.SetActive(false);
        highScoreBody.gameObject.SetActive(false);
        highScoreTitle.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            Debug.Log("WARNING!! DELETING PLAYER PREFS!!!");
            PlayerPrefs.DeleteAll();
        }

        if(!GameManager.Instance.gameOver)
        {
            if (timeElapsed > addToScoreInterval)
            {
                currentScore += scoreIncrement;
                score.text = currentScore.ToString();
                timeElapsed = 0f;
            }
            timeElapsed += Time.deltaTime;
        }
        else
        {
            if(enteringHighScore)
            {
                UpdateHighScorePrompt();
            }
        }
    }

    private void UpdateHighScorePrompt()
    {
        float vert = Input.GetAxisRaw("Vertical");

        if(vert != 0f)
        {
            if(!axisInput)
            {
                axisInput = true;

                if(vert > 0f)
                {
                    if (highScorePromptIndex < alphabet.Length - 1)
                    {
                        highScorePromptIndex++;
                    }
                    else
                    {
                        highScorePromptIndex = 0;
                    }
                }
                else
                {
                    if (highScorePromptIndex > 0)
                    {
                        highScorePromptIndex--;
                    }
                    else
                    {
                        highScorePromptIndex = alphabet.Length - 1;
                    }
                }

                SetCurrentHighScoreNamePrompt();
            }
        }
        else
        {
            axisInput = false;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            currentPromptLetterIndex++;

            if(currentPromptLetterIndex == 3)
            {
                // Done entering high score
                enteringHighScore = false;
                SaveHighScore();
            }
            else
            {
                SetCurrentHighScoreNamePrompt();
            }
        }
        
        float blinkTime = 0.5f;

        if (timeElapsed > blinkTime)
        {
            highScoreBody.text = blinking ? blinkHighScoreName : currentHighScoreName;
            timeElapsed = 0f;
            blinking = !blinking;
        }

        timeElapsed += Time.deltaTime;
    }

    private void SetCurrentHighScoreNamePrompt()
    {
        string currentLetter = alphabet[highScorePromptIndex].ToString();

        currentHighScoreName = currentHighScoreName.Remove(currentPromptLetterIndex, 1).Insert(currentPromptLetterIndex, currentLetter);
        highScoreBody.text = currentHighScoreName;

        blinkHighScoreName = currentHighScoreName.Remove(currentPromptLetterIndex, 1).Insert(currentPromptLetterIndex, "_");

        highScoreBody.text = currentHighScoreName;
    }

    public void ShowAIEliminationBonus(Color eliminatedSignalColor)
    {
        Debug.Log("Showing elimination bonus!");

        currentScore += AIEliminationBonus;
        score.text = currentScore.ToString();
        StartCoroutine(ShowBonusText(eliminatedSignalColor));
    }

    private IEnumerator ShowBonusText(Color c)
    {
        AIEliminationBonusText.gameObject.SetActive(true);
        AIEliminationBonusText.color = new Color(c.r, c.g, c.b);

        yield return new WaitForSeconds(1f);

        AIEliminationBonusText.gameObject.SetActive(false);
    }

    public void AddWinBonus()
    {
        currentScore += winBonus;
        score.text = currentScore.ToString();
        AIEliminationBonusText.gameObject.SetActive(true);
        AIEliminationBonusText.color = Color.blue;
        AIEliminationBonusText.text = "WIN ++500!";
    }

    public void SaveHighScore()
    {
        // Before setting new high score, move everything below the index down 1

        // ADD .... 100
        // MAD .... 50
        // SHR .... 25

        // NEW HIGH SCORE!!! MAD .... 75
        // ---->
        // ADD .... 100
        // MAD .... 75
        // MAD .... 50
        string prevScore; 

        for(int i = numHighScores - 1; i > highScoreRankingIndex; i--)
        {
            if(PlayerPrefs.HasKey((i - 1).ToString()))
            {
                prevScore = PlayerPrefs.GetString((i - 1).ToString());

                if(PlayerPrefs.HasKey(i.ToString()))
                {
                    PlayerPrefs.SetString(i.ToString(), prevScore);
                    currentHighScoresRaw[i] = prevScore;
                }
            }
        }

        string key = highScoreRankingIndex.ToString();
        string body = currentHighScoreName + "....." + currentScore.ToString();
        PlayerPrefs.SetString(key, body);

        currentHighScoresRaw[highScoreRankingIndex] = body;

        highScoreBody.text = "";

        // Show updated high scores
        for(int i = 0; i < currentHighScoresRaw.Length; i++)
        {
            highScoreBody.text += currentHighScoresRaw[i] + "\n";
        }
    }

    public bool IsItAHighScore()
    {
        int[] highScores = GetHighScores();

        for(int i = 0; i < highScores.Length; i++)
        {
            if(currentScore > highScores[i])
            {
                highScoreRankingIndex = i;
                return true;
            }
        }
        return false;
    }

    public int[] GetHighScores()
    {
        string score;
        int[] scores = new int[] { 0, 0, 0, 0, 0};
        int scoreIndex;

        for(int i = 0; i < numHighScores; i++)
        {
            if(PlayerPrefs.HasKey(i.ToString()))
            {
                score = PlayerPrefs.GetString(i.ToString());
                Debug.Log(score);
                currentHighScoresRaw[i] = score;

                // Formatted like ADD....500
                scoreIndex = score.LastIndexOf(".") + 1;
                Debug.Log(score.Substring(scoreIndex));
                scores[i] = int.Parse(score.Substring(scoreIndex));
                Debug.Log("High Score: " + scores[i]);
            }
        }

        return scores;
    }

    public void EnterHighScorePrompt()
    {
        highScoreTitle.gameObject.SetActive(true);
        highScoreTitle.text = "new high score!!!";
        highScoreTitle.color = new Color(0f, 244f, 255f);

        highScoreBody.gameObject.SetActive(true);
        highScoreBody.text = currentHighScoreName;
        highScoreBody.color = Color.yellow;
        enteringHighScore = true;
    }
}
