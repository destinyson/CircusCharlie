using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject star;
    public GameObject menu;
    public int menuBlinkCount;
    public AudioClip applause;

    private bool isStart;
    private float menuBlinkTimeVal;
    private float menuBlinkTime;
    private GameObject am;

    void Start()
    {
        isStart = false;
        menuBlinkTimeVal = GlobalArg.fps * 8;
        menuBlinkTime = 0;
        am = GameObject.Find("AudioManager");

        GlobalArg.playerLife = 3;
        GlobalArg.playerScore = 0;
        GlobalArg.playerStage = 1;
        GlobalArg.playerPassCount = 0;
        GlobalArg.addLifeScore = 20000;
    }

    void Update()
    {
        if (!isStart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isStart = true;
                star.GetComponent<Animator>().enabled = false;
                am.GetComponent<AudioSource>().clip = applause;
                am.GetComponent<AudioSource>().loop = false;
                am.GetComponent<AudioSource>().Play();
            }
        }

        else
        {
            if (menuBlinkCount > 0)
            {
                if (menuBlinkTime <= 0)
                {
                    menu.SetActive(!menu.activeSelf);
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
