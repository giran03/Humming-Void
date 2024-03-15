using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] TMP_Text graffitiCollectedText;
    [SerializeField] TMP_Text highScoreText;

    int currentTotal;
    int highScore;
    void Start()
    {
        LevelSceneManager.Instance.EnableCursor();

        highScore = PlayerPrefs.GetInt("highScore");
        currentTotal = PlayerPrefs.GetInt("graffitiCount");

        // display the total graffiti collected count
        graffitiCollectedText.SetText("X " + currentTotal);

        // sets new high score
        if (currentTotal > highScore)
        {
            PlayerPrefs.SetInt("highScore", currentTotal);
            highScoreText.SetText("X " + currentTotal);
        }
        // display last high score
        else
            highScoreText.SetText("X " + highScore);

        PlayerPrefs.Save();
    }


}
