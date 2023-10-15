using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : Stage
{
    // ��Ȧ����
    public float[] fireRingDis;                     // ��Ȧ��༸�ֿ���ֵ
    public float fireRingNeighborDis;               // �������˫��Ȧ��������Ȧ�ļ��
    public float fireRingWidth;                     // ��Ȧ���
    public float fireRingInitPosX;                  // ��һ����Ȧ��������
    public float fireRingPosY;                      // ��ͨ��Ȧ��������
    public float bonusFireRingPosY;                 // Ǯ����Ȧ��������

    // ��Ȧ�������ز���ʵ��
    public GameObject fireRingPrefab;               // ��ͨ��ȦԤ����
    public GameObject bonusFireRingPrefab;          // Ǯ����ȦԤ����

    private LinkedList<GameObject> fireRingList;    // ��Ȧʵ���б�

    public GameObject[] firePotList;                // ����ʵ���б�

    protected override void Start()
    {
        base.Start();

        fireRingList = new LinkedList<GameObject>();
        // ���ɵ�һ����Ȧ
        createNextFireRing(camera.transform.position.x + fireRingInitPosX);
    }

    protected override void Update()
    {
        base.Update();

        // �����ʤ��ɾ�����л�Ȧ
        if (GlobalArg.isPlayerWin)
        {
            while (fireRingList.Count > 0)
            {
                GameObject obj = fireRingList.First.Value;
                fireRingList.RemoveFirst();
                Destroy(obj);
                obj = null;
            }
        }

        else if (!GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                // ����б����һ����Ȧ������Ļ�Ҳ�ﵽ��Ȧ�����Сֵ�������Ҳ������»�Ȧ
                if (fireRingList.Last.Value.transform.position.x + fireRingDis[2] - fireRingWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2)
                {
                    // ��Ȧ�������������ֻ�Ȧ���Ӵ�С���ʱ�Ϊ 6 ��3 ��1
                    int disRand = Random.Range(0, 10);
                    float dis = 0;
                    if (disRand == 0)
                        dis = fireRingDis[2];
                    else if (disRand <= 3)
                        dis = fireRingDis[1];
                    else
                        dis = fireRingDis[0];
                    // �����»�Ȧ
                    createNextFireRing(fireRingList.Last.Value.transform.position.x + dis);
                }

                // ���б��һ����Ȧ�Ƴ���Ļ�⣬��ɾ����һ����Ȧ
                if (fireRingList.First.Value.transform.position.x + fireRingWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    GameObject ring = fireRingList.First.Value;
                    fireRingList.RemoveFirst();
                    Destroy(ring);
                    ring = null;
                }
            }
        }
    }

    protected override void stopAnim()
    {
        // ֹͣ������綯��
        for (int i = 0; i < firePotList.Length; ++i)
            firePotList[i].transform.Find("fire").gameObject.GetComponent<Animator>().speed = 0;

        // ֹͣ��Ȧ����
        foreach (GameObject ring in fireRingList)
            ring.GetComponent<FireRing>().stop();
    }

    // �÷�����ָ��������λ�����ɻ�Ȧ
    private void createNextFireRing(float posX)
    {
        // ��Ȧ�������������/˫��Ȧ���ʱ� 4 ��1
        int ringNumRand = Random.Range(0, 5);

        if (ringNumRand == 0)
        {
            // ����˫��Ȧ
            // ��Ȧ�������������/С��Ȧ���ʱ� 4 ��1����ͬ
            // Ϊ����˫��Ȧ����С��Ȧ�������Χ������0
            int ringKindRand = Random.Range(1, 25);
            int firstRingKindRand = ringKindRand / 5;
            int secondRingKindRand = ringKindRand % 5;
            // ����������Ȧʵ��
            GameObject ring1 = Instantiate(firstRingKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            GameObject ring2 = Instantiate(secondRingKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            // ָ��λ��
            ring1.transform.position = new Vector3(posX, firstRingKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            ring2.transform.position = new Vector3(posX + fireRingNeighborDis, secondRingKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            // ������Ȧʵ������б�
            fireRingList.AddLast(ring1);
            fireRingList.AddLast(ring2);
        }

        else
        {
            // ���ɵ���Ȧ
            // ��Ȧ���������
            int ringKindRand = Random.Range(0, 5);
            // ���ɻ�Ȧʵ��
            GameObject ring = Instantiate(ringKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            // ָ��λ��
            ring.transform.position = new Vector3(posX, ringKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            // ��Ȧʵ������б�
            fireRingList.AddLast(ring);
        }
    }
}
