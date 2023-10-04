using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject star;
    public GameObject option;
    public GameObject[] menu;
    public int menuBlinkCount;
    public AudioClip applause;

    private float optionPosX;
    private float optionPosY;
    private bool isStart;
    private float menuBlinkTimeVal;
    private float menuBlinkTime;
    private GameObject am;

    void Start()
    {
        optionPosX = option.transform.position.x;
        optionPosY = option.transform.position.y;
        isStart = false;
        menuBlinkTimeVal = GlobalArg.fps * 8;
        menuBlinkTime = 0;
        am = GameObject.Find("AudioManager");

        GlobalArg.playerLife[0] = 3;
        GlobalArg.playerLife[1] = 3;
        GlobalArg.playerScore[0] = 0;
        GlobalArg.playerScore[1] = 0;
        GlobalArg.playerStage[0] = 1;
        GlobalArg.playerStage[1] = 1;
        GlobalArg.playerOrder = 0;

        GlobalArg.playerPosX[0] = GlobalArg.playerInitPosX[0];
        GlobalArg.playerPosX[1] = GlobalArg.playerInitPosX[1];
    }

    void Update()
    {
        if (!isStart)
        {
            if (Input.GetKeyDown(GlobalArg.K_SELECT))
            {
                GlobalArg.mode = (GlobalArg.mode + 1) % 4;
                option.transform.position = new Vector3(optionPosX, optionPosY - 0.32f * GlobalArg.mode, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                isStart = true;
                star.GetComponent<Animator>().enabled = false;
                am.GetComponent<AudioManager>().playAudioClip(applause, false);
            }
        }

        else
        {
            if (menuBlinkCount > 0)
            {
                if (menuBlinkTime <= 0)
                {
                    menu[GlobalArg.mode].SetActive(!menu[GlobalArg.mode].activeSelf);
                    menuBlinkTime = menuBlinkTimeVal;
                    --menuBlinkCount;
                }
                else
                    menuBlinkTime -= Time.deltaTime;
            }
            else
                SceneManager.LoadScene(1);
        }
    }
}
