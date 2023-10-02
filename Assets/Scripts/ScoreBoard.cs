using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public Vector2 playerIconPos;
    public Sprite playerIcon;
    public bool isCount;

    public Text playerScoreText;
    public Text hiScoreText;
    public Text playerStageText;
    public Text timeText;

    private GameObject boardCanvas;
    void Start()
    {
        playerStageText.text = "STAGE-" + GlobalArg.playerStage[GlobalArg.playerOrder].ToString().PadLeft(2, '0');

        for (int i = 0; i < GlobalArg.playerLife[GlobalArg.playerOrder] + (isCount ? 1 : 0); ++i)
        {
            GameObject playerIconImg = new GameObject();
            playerIconImg.transform.SetParent(transform);
            playerIconImg.AddComponent<SpriteRenderer>();
            playerIconImg.transform.position = new Vector3(playerIconPos.x - 0.32f * i, playerIconPos.y) + transform.position;
            playerIconImg.GetComponent<SpriteRenderer>().sprite = playerIcon;
        }
    }

    void Update()
    {
        playerScoreText.text = (GlobalArg.playerOrder + 1).ToString() + "P-" + GlobalArg.playerScore[GlobalArg.playerOrder].ToString().PadLeft(6, '0');
        hiScoreText.text = "HI-" + GlobalArg.hiScore.ToString().PadLeft(6, '0');
        timeText.text = "-" + GlobalArg.time.ToString().PadLeft(4, '0');
    }
}
