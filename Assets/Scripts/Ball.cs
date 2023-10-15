using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // 皮球信息
    public float scrollSpeed;       // 皮球慢慢滚动速度
    public float moveSpeed;         // 皮球借力移动速度

    public enum ballState { scroll, stand, forward, backward, backrun, forrun };    // 皮球状态种类
    public ballState state;                                                         // 皮球状态

    private GameObject player;      // 玩家实体

    void Awake()
    {
        // 皮球状态初始化为慢慢滚动
        state = ballState.scroll;

        // 获取玩家实体
        player = GameObject.Find("player");
    }
    
    void Update()
    {
        switch (state)
        {
            // 前三种状态皮球跟随玩家
            case ballState.stand: 
            case ballState.forward: 
            case ballState.backward: transform.position = new Vector3(player.transform.position.x, transform.position.y); break;
            // 皮球慢慢滚动
            case ballState.scroll: transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime); break;
            // 皮球向后快速滚动
            case ballState.backrun: transform.Translate(Vector2.left * moveSpeed * Time.deltaTime); break;
            // 皮球向前快速滚动
            case ballState.forrun: transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); break;
            default: break;
        }
        
        // 皮球动画设置
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
            // 皮球快速滚动时设置物体所在层保证不与其他物体碰撞
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
        // 两皮球相撞
        if (collision.tag == "Ball" && gameObject.layer != LayerMask.NameToLayer("Remains") &&
            collision.gameObject.transform.parent.gameObject.layer != LayerMask.NameToLayer("Remains"))
        {
            // 如果是从右边撞的，自身向后滚，对方向前滚
            if (collision.bounds.ClosestPoint(transform.position).x > 0)
            {
                state = ballState.backrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.forrun;
            }
            // 否则自身向前滚，对方向后滚
            else
            {
                state = ballState.forrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.backrun;
            }

            // 设置物体所在层保证不与其他物体碰撞
            gameObject.layer = LayerMask.NameToLayer("Remains");
            collision.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Remains");
        }
    }
}
