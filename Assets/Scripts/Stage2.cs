using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2 : Stage
{
    // 猴子参数
    public float[] monkeyDis;                   // 猴子间距
    public float monkeyNeighborDis;             // 如果生成双猴子，两个猴子的间距
    public float bonusMonkeyDis;                // 奖励猴子生成时与普通猴子的间距
    public float monkeyWidth;                   // 猴子宽度        
    public float monkeyInitPosX;                // 第一个猴子横向坐标
    public float monkeyPosY;                    // 猴子纵向坐标

    private bool createBonus;                   // 判断本次是否生成奖励猴子
    private bool createDouble;                  // 记录上次是否一次生成两个猴子

    // 猴子素材与实体
    public GameObject monkeyPrefab;             // 普通猴子预制体
    public GameObject bonusMonkeyPrefab;        // 奖励猴子预制体

    private LinkedList<GameObject> monkeyList;  // 猴子实体列表

    // 掉落音效
    public AudioClip drop;

    protected override void Start()
    {
        base.Start();

        monkeyList = new LinkedList<GameObject>();
        // 生成第一个猴子
        int monkeyKindRand = UnityEngine.Random.Range(0, 10);
        createNextMonkey(camera.transform.position.x + monkeyInitPosX, monkeyKindRand == 0);
    }

    protected override void Update()
    {
        base.Update();

        // 如果获胜，删除所有猴子
        if (GlobalArg.isPlayerWin)
        {
            while (monkeyList.Count > 0)
            {
                GameObject obj = monkeyList.First.Value;
                monkeyList.RemoveFirst();
                Destroy(obj);
                obj = null;
            }
        }

        else if (GlobalArg.isPlayerDrop)
        {
            if (am.GetComponent<AudioSource>().clip == BGM || am.GetComponent<AudioSource>().clip == warningBGM)
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
                    am.GetComponent<AudioSource>().clip = drop;
                    am.GetComponent<AudioSource>().loop = false;
                    am.GetComponent<AudioSource>().Play();
                }
            }
        }

        else if (!GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                if (!createBonus)
                {
                    if (monkeyList.Last.Value.transform.position.x + monkeyDis[0] - monkeyWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2)
                    {
                        // 猴子种类随机数，普通/奖励概率比 9 ：1，下同
                        int monkeyKindRand = UnityEngine.Random.Range(0, 10);

                        // 奖励猴子不能连续生成，上一次生成普通猴子本次才可能生成奖励猴子
                        // 如果本次生成奖励猴子，要等一会，等到列表最后一个猴子距离屏幕右侧达到阈值再进行生成
                        if (!monkeyList.Last.Value.GetComponent<Monkey>().isBonus && !createDouble && monkeyKindRand == 0)
                            createBonus = true;
                        
                        // 否则生成普通猴子 
                        else
                        {
                            int disRand = Random.Range(0, 144);
                            float dis = monkeyDis[(int)Mathf.Sqrt(disRand)];
                            createNextMonkey(monkeyList.Last.Value.transform.position.x + dis, false);
                        }
                    }
                    
                }

                else
                {
                    if (monkeyList.Last.Value.transform.position.x + bonusMonkeyDis - monkeyWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2)
                    {
                        // 生成奖励猴子
                        createNextMonkey(monkeyList.Last.Value.transform.position.x + bonusMonkeyDis, true);
                        createBonus = false;
                    }
                }

                // 若列表第一个猴子移出屏幕外，则删除第一个猴子
                if (monkeyList.First.Value.transform.position.x + monkeyWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    GameObject monkey = monkeyList.First.Value;
                    monkeyList.RemoveFirst();
                    Destroy(monkey);
                    monkey = null;
                }
            }
        }
    }

    protected override void stopAnim()
    {
        // 停止猴子动画
        foreach (GameObject monkey in monkeyList)
        {
            // 如果是奖励猴子，将重力与速度设为0以停止跳跃
            if (monkey.GetComponent<Monkey>().isBonus)
            {
                monkey.GetComponent<Rigidbody2D>().gravityScale = 0;
                monkey.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            // 停止动画
            monkey.GetComponent<Animator>().speed = 0;
        }
    }

    // 该方法在指定横坐标位置生成猴子
    private void createNextMonkey(float posX, bool thisBonus)
    {
        // 猴子数量随机数，单/双猴子概率比 9 ：1
        int monkeyNumRand = Random.Range(0, 10);

        if (monkeyNumRand == 0)
        {
            // 生成双猴子
            createDouble = true;
            // 生成两个猴子实体
            GameObject monkey1 = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            GameObject monkey2 = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            // 指定位置
            monkey1.transform.position = new Vector3(posX, monkeyPosY);
            monkey2.transform.position = new Vector3(posX + monkeyNeighborDis, monkeyPosY);
            // 两个猴子实体加入列表
            monkeyList.AddLast(monkey1);
            monkeyList.AddLast(monkey2);
        }

        else
        {
            // 生成单猴子
            createDouble = false;
            // 生成猴子实体
            GameObject monkey = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            // 指定位置
            monkey.transform.position = new Vector3(posX, monkeyPosY);
            // 猴子实体加入列表
            monkeyList.AddLast(monkey);
        }
    }
}
