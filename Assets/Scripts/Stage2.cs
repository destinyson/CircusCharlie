using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2 : Stage
{
    // ���Ӳ���
    public float[] monkeyDis;                   // ���Ӽ��
    public float monkeyNeighborDis;             // �������˫���ӣ��������ӵļ��
    public float bonusMonkeyDis;                // ������������ʱ����ͨ���ӵļ��
    public float monkeyWidth;                   // ���ӿ��        
    public float monkeyInitPosX;                // ��һ�����Ӻ�������
    public float monkeyPosY;                    // ������������

    private bool createBonus;                   // �жϱ����Ƿ����ɽ�������
    private bool createDouble;                  // ��¼�ϴ��Ƿ�һ��������������

    // �����ز���ʵ��
    public GameObject monkeyPrefab;             // ��ͨ����Ԥ����
    public GameObject bonusMonkeyPrefab;        // ��������Ԥ����

    private LinkedList<GameObject> monkeyList;  // ����ʵ���б�

    // ������Ч
    public AudioClip drop;

    protected override void Start()
    {
        base.Start();

        monkeyList = new LinkedList<GameObject>();
        // ���ɵ�һ������
        int monkeyKindRand = UnityEngine.Random.Range(0, 10);
        createNextMonkey(camera.transform.position.x + monkeyInitPosX, monkeyKindRand == 0);
    }

    protected override void Update()
    {
        base.Update();

        // �����ʤ��ɾ�����к���
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
                        // �����������������ͨ/�������ʱ� 9 ��1����ͬ
                        int monkeyKindRand = UnityEngine.Random.Range(0, 10);

                        // �������Ӳ����������ɣ���һ��������ͨ���ӱ��βſ������ɽ�������
                        // ����������ɽ������ӣ�Ҫ��һ�ᣬ�ȵ��б����һ�����Ӿ�����Ļ�Ҳ�ﵽ��ֵ�ٽ�������
                        if (!monkeyList.Last.Value.GetComponent<Monkey>().isBonus && !createDouble && monkeyKindRand == 0)
                            createBonus = true;
                        
                        // ����������ͨ���� 
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
                        // ���ɽ�������
                        createNextMonkey(monkeyList.Last.Value.transform.position.x + bonusMonkeyDis, true);
                        createBonus = false;
                    }
                }

                // ���б��һ�������Ƴ���Ļ�⣬��ɾ����һ������
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
        // ֹͣ���Ӷ���
        foreach (GameObject monkey in monkeyList)
        {
            // ����ǽ������ӣ����������ٶ���Ϊ0��ֹͣ��Ծ
            if (monkey.GetComponent<Monkey>().isBonus)
            {
                monkey.GetComponent<Rigidbody2D>().gravityScale = 0;
                monkey.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            // ֹͣ����
            monkey.GetComponent<Animator>().speed = 0;
        }
    }

    // �÷�����ָ��������λ�����ɺ���
    private void createNextMonkey(float posX, bool thisBonus)
    {
        // �����������������/˫���Ӹ��ʱ� 9 ��1
        int monkeyNumRand = Random.Range(0, 10);

        if (monkeyNumRand == 0)
        {
            // ����˫����
            createDouble = true;
            // ������������ʵ��
            GameObject monkey1 = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            GameObject monkey2 = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            // ָ��λ��
            monkey1.transform.position = new Vector3(posX, monkeyPosY);
            monkey2.transform.position = new Vector3(posX + monkeyNeighborDis, monkeyPosY);
            // ��������ʵ������б�
            monkeyList.AddLast(monkey1);
            monkeyList.AddLast(monkey2);
        }

        else
        {
            // ���ɵ�����
            createDouble = false;
            // ���ɺ���ʵ��
            GameObject monkey = Instantiate(thisBonus ? bonusMonkeyPrefab : monkeyPrefab, transform);
            // ָ��λ��
            monkey.transform.position = new Vector3(posX, monkeyPosY);
            // ����ʵ������б�
            monkeyList.AddLast(monkey);
        }
    }
}
