using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public GameObject[] playerIcon;
    public bool isCount;

    public Text playerScoreText;
    public Text hiScoreText;
    public Text playerStageText;
    public Text timeText;

    void Start()
    {
        playerStageText.text = "STAGE-" + GlobalArg.playerStage.ToString().PadLeft(2, '0');
    }

    void Update()
    {
        playerScoreText.text = "1P-" + GlobalArg.playerScore.ToString().PadLeft(6, '0');
        hiScoreText.text = "HI-" + GlobalArg.hiScore.ToString().PadLeft(6, '0');
        timeText.text = "-" + GlobalArg.time.ToString().PadLeft(4, '0');

        for (int i = 0; i < Mathf.Min(GlobalArg.playerLife + (isCount ? 1 : 0), 6); ++i)
            playerIcon[i].SetActive(true);
        for (int i = Mathf.Min(GlobalArg.playerLife + (isCount ? 1 : 0), 6); i < 6; ++i)
            playerIcon[i].SetActive(false);
    }
}
