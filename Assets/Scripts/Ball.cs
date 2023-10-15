using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Ƥ����Ϣ
    public float scrollSpeed;       // Ƥ�����������ٶ�
    public float moveSpeed;         // Ƥ������ƶ��ٶ�

    public enum ballState { scroll, stand, forward, backward, backrun, forrun };    // Ƥ��״̬����
    public ballState state;                                                         // Ƥ��״̬

    private GameObject player;      // ���ʵ��

    void Awake()
    {
        // Ƥ��״̬��ʼ��Ϊ��������
        state = ballState.scroll;

        // ��ȡ���ʵ��
        player = GameObject.Find("player");
    }
    
    void Update()
    {
        switch (state)
        {
            // ǰ����״̬Ƥ��������
            case ballState.stand: 
            case ballState.forward: 
            case ballState.backward: transform.position = new Vector3(player.transform.position.x, transform.position.y); break;
            // Ƥ����������
            case ballState.scroll: transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime); break;
            // Ƥ�������ٹ���
            case ballState.backrun: transform.Translate(Vector2.left * moveSpeed * Time.deltaTime); break;
            // Ƥ����ǰ���ٹ���
            case ballState.forrun: transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); break;
            default: break;
        }
        
        // Ƥ�򶯻�����
        switch (state)
        {
            case ballState.stand:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", 0);
                    break;
                }
            case ballState.forward:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", 1);
                    break;
                }
            case ballState.backward:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", -1);
                    break;
                }
            // Ƥ����ٹ���ʱ�����������ڲ㱣֤��������������ײ
            case ballState.backrun:
                {
                    GetComponent<Animator>().SetTrigger("kick");
                    GetComponent<Animator>().SetInteger("h", -1);
                    gameObject.layer = LayerMask.NameToLayer("Remains");
                    break;
                }
            case ballState.forrun:
                {
                    GetComponent<Animator>().SetTrigger("kick");
                    GetComponent<Animator>().SetInteger("h", 1);
                    gameObject.layer = LayerMask.NameToLayer("Remains");
                    break;
                }
            default: break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��Ƥ����ײ
        if (collision.tag == "Ball" && gameObject.layer != LayerMask.NameToLayer("Remains") &&
            collision.gameObject.transform.parent.gameObject.layer != LayerMask.NameToLayer("Remains"))
        {
            // ����Ǵ��ұ�ײ�ģ������������Է���ǰ��
            if (collision.bounds.ClosestPoint(transform.position).x > 0)
            {
                state = ballState.backrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.forrun;
            }
            // ����������ǰ�����Է�����
            else
            {
                state = ballState.forrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.backrun;
            }

            // �����������ڲ㱣֤��������������ײ
            gameObject.layer = LayerMask.NameToLayer("Remains");
            collision.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Remains");
        }
    }
}
