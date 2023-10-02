using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountUI : MonoBehaviour
{
    public float changeSceneTime;
    public Text announceText;

    void Start()
    {
        Time.timeScale = 1;
        if (GlobalArg.isPlayerWin)
            ++GlobalArg.playerStage[GlobalArg.playerOrder];
        else if (GlobalArg.isPlayerDie)
            --GlobalArg.playerLife[GlobalArg.playerOrder];
        GlobalArg.isPlayerWin = false;
        GlobalArg.isPlayerDie = false;
        GlobalArg.time = 5000;

        announceText.text = GlobalArg.playerLife[GlobalArg.playerOrder] < 0 ?
                            "GAME OVER" : ("STAGE " + GlobalArg.playerStage[GlobalArg.playerOrder].ToString().PadLeft(2, '0'));

        if (GlobalArg.playerLife[GlobalArg.playerOrder] < 0)
            Invoke("changeMenuScene", changeSceneTime);
        else
            Invoke("changeGameScene", changeSceneTime);
    }

    void changeMenuScene() { SceneManager.LoadScene(0); }
    void changeGameScene() { SceneManager.LoadScene(2); }
}
