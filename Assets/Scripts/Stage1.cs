using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : MonoBehaviour
{
    public GameObject camera;
    public GameObject player;
    public GameObject fireRingPrefab;
    public GameObject bonusFireRingPrefab;
    public Animator[] audienceAnim;
    public float bigFireRingDis;
    public float smallFireRingDis;
    public float fireRingWidth;
    public float fireRingPosY;
    public float bonusFireRingPosY;
    public AudioClip BGM1;
    public AudioClip warningBGM1;
    public AudioClip die;
    public AudioClip gameOver;
    public AudioClip applause;
    public AudioClip score;
    public AudioClip pauseClip;
    public GameObject pauseText;

    private AudioClip[] loseClipList;
    private GameObject oldFireRing;
    private GameObject newFireRing;
    private GameObject am;
    private bool startWarning;
    private float timeLessVal;
    private float timeLess;
    private bool isPause;
    private float pauseInitPosX;
    private float pauseInitPosY;

    void Start()
    {
        camera.transform.position = new Vector3(player.transform.position.x + 2.88f, 0, -10);
        oldFireRing = Instantiate(fireRingPrefab, transform);
        oldFireRing.transform.position = new Vector3(camera.transform.position.x + 5.04f, -0.68f);
        newFireRing = null;
        startWarning = false;
        timeLessVal = GlobalArg.fps * 16;
        timeLess = timeLessVal;
        am = GameObject.Find("AudioManager");
        loseClipList = new AudioClip[] { die, gameOver };
        isPause = false;
        pauseText.SetActive(false);
        pauseInitPosX = pauseText.transform.position.x;
        pauseInitPosY = pauseText.transform.position.y;
        am.GetComponent<AudioManager>().playAudioClip(BGM1, true);
    }

    void Update()
    {
        if (GlobalArg.isPlayerWin)
        {
            if (oldFireRing)
            {
                Destroy(oldFireRing);
                oldFireRing = null;
            }
            if (newFireRing)
            {
                Destroy(newFireRing);
                newFireRing = null;
            }

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
                    AudioSource.PlayClipAtPoint(pauseClip, transform.position);
                    pauseText.transform.position = new Vector3(pauseInitPosX + camera.transform.position.x,
                                                               pauseInitPosY + camera.transform.position.y);
                    pauseText.SetActive(true);
                }

                isPause = !isPause;
            }

            else
            {
                if (GlobalArg.time <= GlobalArg.warningTime)
                {
                    if (!startWarning)
                    {
                        am.GetComponent<AudioManager>().playAudioClip(warningBGM1, true);
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

                if (oldFireRing.transform.position.x + smallFireRingDis + fireRingWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2 && !newFireRing)
                {
                    int disRand = Random.Range(0, 10);
                    int kindRand = Random.Range(0, 10);
                    newFireRing = Instantiate(kindRand == 0 ? bonusFireRingPrefab : fireRingPrefab,
                                              new Vector3(oldFireRing.transform.position.x + (disRand < 3 ? smallFireRingDis : bigFireRingDis),
                                                          kindRand == 0 ? bonusFireRingPosY : fireRingPosY),
                                              Quaternion.identity, transform);
                }

                else if (oldFireRing.transform.position.x + fireRingWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    Destroy(oldFireRing);
                    oldFireRing = newFireRing;
                    newFireRing = null;
                }
            }
        }
    }
}
