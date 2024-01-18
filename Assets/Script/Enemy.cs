using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Vector3 mousePosition;
    [SerializeField] Pathfinding path;

    public Camera cam;

    Animator anim;
    List<Node> myWay;
    Node targetNode;
    bool trigger = true;
    void Start()
    {
        anim = GetComponent<Animator>();
    }



    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 targetPos;
            targetPos = Collision_Handler.instance.player.rigid.position;
            targetPos = new Vector3(targetPos.x, targetPos.y, 0);
            
            List<Node> newWay = path.PathFind(transform.position, targetPos);
            if(newWay != null)
            {
                StopCoroutine("move");
                myWay = newWay;
                StartCoroutine("move");
            }
                
            

        }

    }
    IEnumerator move()
    {
        int idx = 0;
        targetNode = myWay[0]; //시작 점
        while (true)
        {
            if (transform.position == targetNode.myPos) //내위치가 타켓 노드까지 왔다면,
            //새로운 타겟 노드를 설정해줘야한다.
            {
                idx++; //인덱스 값을 올려서 다음 경로의 노드 탐색
                if (idx >= myWay.Count) //만약 인덱스값이 경로의 개수와 같아진다면, 도착했음을 뜻함
                {
                    yield break; //빠져나와준다.
                }
                targetNode = myWay[idx];
                if (transform.position.x < targetNode.myPos.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (transform.position.x > targetNode.myPos.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, targetNode.myPos, Time.fixedDeltaTime);
            //타겟 노드까지 이동해준다.
            yield return null;
        }
    }

}