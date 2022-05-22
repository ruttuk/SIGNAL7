using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI score;
    [SerializeField]
    TextMeshProUGUI AIEliminationBonusText;

    int currentScore = 0;
    int scoreIncrement = 25;
    int AIEliminationBonus = 500;

    float timeElapsed = 0f;
    int addToScoreInterval = 1;

    private void Awake()
    {
        AIEliminationBonusText.gameObject.SetActive(false);
    }

    private void Update()
    {
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
}
