using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Stage3 : Stage
{
    public GameObject ballPrefab;
    public Vector2 ballInitPos;
    public Vector2 firstBallPos;
    public float ballWidth;

    private LinkedList<GameObject> ballList;

    protected override void Start()
    {
        base.Start();

        GameObject ball = Instantiate(ballPrefab, transform);
        ball.transform.position = new Vector3(camera.transform.position.x + ballInitPos.x, ballInitPos.y);
        ball.GetComponent<Ball>().state = Ball.ballState.stand;
        ball.GetComponent<Ball>().dir = 0;
        ballList = new LinkedList<GameObject>();
        ballList.AddLast(ball);

        GameObject firstBall = Instantiate(ballPrefab, transform);
        firstBall.transform.position = new Vector3(camera.transform.position.x + firstBallPos.x, firstBallPos.y);
        ballList.AddLast(firstBall);

        loseClipList = new AudioClip[] { die, gameOver };
    }

    protected override void Update()
    {
        base.Update();

        if (GlobalArg.isPlayerWin)
        {
            while (ballList.Count > 0)
            {
                GameObject obj = ballList.First.Value;
                ballList.RemoveFirst();
                Destroy(obj);
                obj = null;
            }
        }

        else if (!GlobalArg.isPlayerWin && !GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                if (ballList.Last.Value.transform.position.x + 1.4f <= camera.transform.position.x + GlobalArg.window_width / 2)
                {
                    
                    int rand = UnityEngine.Random.Range(1, 6);
                    float dis = rand * 0.8f + 1.04f;
                    GameObject ball = Instantiate(ballPrefab, transform);
                    ball.transform.position = new Vector3(ballList.Last.Value.transform.position.x + dis, ballInitPos.y);
                    ballList.AddLast(ball);
                }

                GameObject obj = ballList.First.Value;
                if (obj.transform.position.x + ballWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    ballList.RemoveFirst();
                    Destroy(obj);
                    obj = null;
                }
            }
        }
    }
}
