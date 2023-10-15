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
    public AudioClip addLife;           // 加命音效

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
        // 摄像机与音频管理器设定
        camera = GameObject.Find("Main Camera");
        am = GameObject.Find("AudioManager");
        am.GetComponent<AudioSource>().clip = BGM;
        am.GetComponent<AudioSource>().loop = true;
        am.GetComponent<AudioSource>().Play();

        // 计分板时间设定
        timeLessVal = GlobalArg.fps * 16;
        timeLess = timeLessVal;

        // 警告状态初始化为false
        startWarning = false;

        // 暂停文本设定，设置为隐藏
        pauseText.SetActive(false);
        isPause = false;
        pauseInitPosX = pauseText.transform.position.x;
        pauseInitPosY = pauseText.transform.position.y;
    }

    protected virtual void Update()
    {
        if (GlobalArg.playerScore >= GlobalArg.addLifeScore)
        {
            GlobalArg.addLifeScore += 40000;
            ++GlobalArg.playerLife;
            AudioSource.PlayClipAtPoint(addLife, transform.position);
        }

        // 若玩家获胜
        if (GlobalArg.isPlayerWin)
        {
            // 观众席播放鼓掌动画
            for (int i = 0; i < audienceAnim.Length; ++i)
                audienceAnim[i].SetTrigger("win");

            // 将剩余时间计分
            if (GlobalArg.time > 0)
            {
                GlobalArg.time -= 10;
                GlobalArg.playerScore += 10;
                if (GlobalArg.hiScore < GlobalArg.playerScore)
                    GlobalArg.hiScore = GlobalArg.playerScore;
            }

            // 切换音效
            if (am.GetComponent<AudioSource>().clip == BGM || am.GetComponent<AudioSource>().clip == warningBGM)
            {
                am.GetComponent<AudioSource>().Stop();
                am.GetComponent<AudioSource>().clip = applause;
                am.GetComponent<AudioSource>().loop = false;
                am.GetComponent<AudioSource>().Play();
            }
                

            else if (!am.GetComponent<AudioSource>().isPlaying)
            {
                if (GlobalArg.time == 0)
                    SceneManager.LoadScene(1);
                else
                {
                    am.GetComponent<AudioSource>().clip = score;
                    am.GetComponent<AudioSource>().loop = false;
                    am.GetComponent<AudioSource>().Play();
                }
            }
        }

        // 若玩家死亡
        else if (GlobalArg.isPlayerDie)
        {
            // 暂停指定动画
            stopAnim();
            // 播放相应音效
            if (am.GetComponent<AudioSource>().clip != die && am.GetComponent<AudioSource>().clip != gameOver)
            {
                am.GetComponent<AudioSource>().Stop();
                am.GetComponent<AudioSource>().clip = die;
                am.GetComponent<AudioSource>().loop = false;
                am.GetComponent<AudioSource>().Play();
            }
                
            else if (!am.GetComponent<AudioSource>().isPlaying) 
            {
                if (am.GetComponent<AudioSource>().clip == die)
                {
                    am.GetComponent<AudioSource>().clip = gameOver;
                    am.GetComponent<AudioSource>().loop = false;
                    am.GetComponent<AudioSource>().Play();
                }
                else
                    SceneManager.LoadScene(1);
            }
        }

        // 若玩家掉落
        else if (GlobalArg.isPlayerDrop)
        {
            // 暂停指定动画
            stopAnim();
        }

        // 否则，玩家处于游玩状态
        else
        {
            // 暂停功能设定
            if (Input.GetKeyDown(GlobalArg.K_PAUSE))
            {
                if (isPause)
                {
                    Time.timeScale = 1;
                    am.GetComponent<AudioSource>().Play();
                    pauseText.SetActive(false);
                }

                else
                {
                    Time.timeScale = 0;
                    am.GetComponent<AudioSource>().Pause();
                    AudioSource.PlayClipAtPoint(pause, transform.position);
                    pauseText.transform.position = new Vector3(pauseInitPosX + camera.transform.position.x,
                                                               pauseInitPosY + camera.transform.position.y);
                    pauseText.SetActive(true);
                }

                isPause = !isPause;
            }

            // 如果没有暂停，时间正常流逝
            if (!isPause)
            {
                if (GlobalArg.time <= GlobalArg.warningTime)
                {
                    if (!startWarning)
                    {
                        am.GetComponent<AudioSource>().Stop();
                        am.GetComponent<AudioSource>().clip = warningBGM;
                        am.GetComponent<AudioSource>().loop = true;
                        am.GetComponent<AudioSource>().Play();
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

    protected virtual void stopAnim() { }
}
