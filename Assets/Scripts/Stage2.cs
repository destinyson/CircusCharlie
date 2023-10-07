using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2 : Stage
{
    public GameObject monkeyPrefab;
    public GameObject bonusMonkeyPrefab;
    public Vector2 monkeyInitPos;
    public float monkeyWidth;
    public float monkeyMinDis;
    public float bonusDis;

    private GameObject monkey;
    private LinkedList<GameObject> monkeyList;
    private bool createBonus;

    public AudioClip drop;
    
    protected override void Start()
    {
        base.Start();

        monkeyList = new LinkedList<GameObject>();
        monkey = Instantiate(monkeyPrefab, transform);
        monkey.transform.position = new Vector3(camera.transform.position.x + monkeyInitPos.x, monkeyInitPos.y);
        monkeyList.AddLast(monkey);
        createBonus = false;

        // Ê§°ÜÒôÐ§ÐòÁÐ
        loseClipList = new AudioClip[] { die, drop, die, gameOver };
    }

    protected override void Update()
    {
        base.Update();

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

        else if (!GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                if (!createBonus)
                {
                    if (monkeyList.Last.Value.transform.position.x + monkeyMinDis <= camera.transform.position.x + GlobalArg.window_width / 2)
                    {
                        if (!monkeyList.Last.Value.GetComponent<Monkey>().isBonus && UnityEngine.Random.Range(0, 20) == 0)
                            createBonus = true;
                        else
                        {
                            int rand = UnityEngine.Random.Range(0, 210);
                            float dis = (18 - (int)((71 - Math.Sqrt(5041 - 24 * rand)) / 6)) * 0.32f;
                            monkey = Instantiate(monkeyPrefab, transform);
                            monkey.transform.position = new Vector3(monkeyList.Last.Value.transform.position.x + dis, monkeyInitPos.y);
                            monkeyList.AddLast(monkey);
                        }
                    }
                    
                }

                else
                {
                    if (monkeyList.Last.Value.transform.position.x + bonusDis <= camera.transform.position.x + GlobalArg.window_width / 2)
                    {
                        monkey = Instantiate(bonusMonkeyPrefab, transform);
                        monkey.transform.position = new Vector3(monkeyList.Last.Value.transform.position.x + bonusDis + monkeyWidth / 2, monkeyInitPos.y);
                        monkeyList.AddLast(monkey);
                        createBonus = false;
                    }
                }

                GameObject obj = monkeyList.First.Value;
                if (obj.transform.position.x + monkeyWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    monkeyList.RemoveFirst();
                    Destroy(obj);
                    obj = null;
                }
            }
        }
    }
}
