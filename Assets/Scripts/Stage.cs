using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage : MonoBehaviour
{
    // 实体
    public GameObject player;           // 玩家实体

    protected GameObject camera;        // 摄像机实体
    protected GameObject am;            // AudioManager实体

    // 观众席动画
    public Animator[] audienceAnim;

    // 音效
    public AudioClip BGM;               // BGM
    public AudioClip die;               // 死亡音效
    public AudioClip gameOver;          // 游戏结束音效
    public AudioClip applause;          // 鼓掌音效
    public AudioClip score;             // 计分音效

    protected AudioClip[] loseClipList; // 失败音效序列

    // 时间流逝
    protected float timeLessVal;        // 多少现实时间减少一次游戏时间
    protected float timeLess;           // 上个变量对应功能计时器

    // 警告
    public AudioClip warningBGM;        // 警告BGM

    protected bool startWarning;        // 是否开始警告

    // 暂停文本
    public GameObject pauseText;        // 暂停文本实体
    public AudioClip pause;             // 暂停音效

    protected bool isPause;             // 是否暂停
    protected float pauseInitPosX;      // 暂停文本初始横向坐标
    protected float pauseInitPosY;      // 暂停文本初始纵向坐标

    protected virtual void Start()
    {
        camera = GameObject.Find("Main Camera");
        am = GameObject.Find("AudioManager");
        am.GetComponent<AudioManager>().playAudioClip(BGM, true);

        timeLessVal = GlobalArg.fps * 16;
        timeLess = timeLessVal;

        startWarning = false;

        pauseText.SetActive(false);
        isPause = false;
        pauseInitPosX = pauseText.transform.position.x;
        pauseInitPosY = pauseText.transform.position.y;
    }

    protected virtual void Update()
    {
        if (GlobalArg.isPlayerWin)
        {
            for (int i = 0; i < audienceAnim.Length; ++i)
                audienceAnim[i].SetTrigger("win");

            if (GlobalArg.time > 0)
            {
                GlobalArg.time -= 10;
                GlobalArg.playerScore[GlobalArg.playerOrder] += 10;
            }

            if (am.GetComponent<AudioManager>().isLoop())
                am.GetComponent<AudioManager>().playAudioClip(applause, false);

            else if (!am.GetComponent<AudioManager>().isPlaying())
            {
                if (GlobalArg.time == 0)
                    SceneManager.LoadScene(1);
                else
                    am.GetComponent<AudioManager>().playAudioClip(score, false);
            }
        }

        else if (GlobalArg.isPlayerDie)
        {
            Time.timeScale = 0;
            if (!am.GetComponent<AudioManager>().playAudioClipFromList(loseClipList) && !am.GetComponent<AudioManager>().isPlaying())
                SceneManager.LoadScene(1);
        }

        else
        {
            if (Input.GetKeyDown(GlobalArg.K_PAUSE))
            {
                if (isPause)
                {
                    Time.timeScale = 1;
                    am.GetComponent<AudioManager>().play();
                    pauseText.SetActive(false);
                }

                else
                {
                    Time.timeScale = 0;
                    am.GetComponent<AudioManager>().pause();
                    AudioSource.PlayClipAtPoint(pause, transform.position);
                    pauseText.transform.position = new Vector3(pauseInitPosX + camera.transform.position.x,
                                                               pauseInitPosY + camera.transform.position.y);
                    pauseText.SetActive(true);
                }

                isPause = !isPause;
            }

            if (!isPause)
            {
                if (GlobalArg.time <= GlobalArg.warningTime)
                {
                    if (!startWarning)
                    {
                        am.GetComponent<AudioManager>().playAudioClip(warningBGM, true);
                        startWarning = true;
                    }
                }

                if (timeLess <= 0)
                {
                    GlobalArg.time -= 10;
                    timeLess = timeLessVal;
                }
                else
                    timeLess -= Time.deltaTime;
            }
        }
    }
}
