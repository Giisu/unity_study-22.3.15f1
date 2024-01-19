using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Vector3 mousePosition;
    [SerializeField] Pathfinding path;

    public Camera cam;
    Rigidbody2D rigid;

    Animator anim;
    List<Node> myWay;
    Node targetNode;
    Node curNode;
    public Vector2 nextVec = new Vector2(0,0);

    BoxCollider2D coll;
    bool trigger = true;
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
    }
    //public List<int[,]> ints = new List<int[,]>();


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Follow();
        Collision2.instance.GetPlayerColl();
        Collision2.instance.CollisionCheck();
        Repos();
        
    }

    public void Repos()
    {
        rigid.velocity = nextVec * Collision2.instance.horiVec.x * Collision2.instance.vertVec.y;
        Debug.Log(nextVec * Collision2.instance.horiVec.x * Collision2.instance.vertVec.y);

    }
    void Follow()
    {
        
        
            Vector3 targetPos;
            targetPos = Collision_Handler.instance.player.rigid.position;
            targetPos = new Vector3(targetPos.x, targetPos.y, 0);
            
            List<Node> newWay = path.PathFind(rigid.position, targetPos);
            if(newWay != null)
            {
                StopCoroutine("move");
                myWay = newWay;
                StartCoroutine("move");
            }
                
            

        

    }
    IEnumerator move()
    {
        int idx = 0;
        targetNode = myWay[0]; //시작 점
        curNode = myWay[0];
        while (true)
        {
            curNode = Grids.instance.GetNodeFromVector(rigid.position);
            nextVec = NextVec(curNode, targetNode);
            yield return nextVec;
            Debug.Log("타겟x"+targetNode.myX);
                Debug.Log("타겟y"+targetNode.myY);
                Debug.Log("현재x"+curNode.myX);
                Debug.Log("현재Y"+curNode.myY);
            if (curNode == targetNode) 
            {
                
                idx++;
                
                
                if (idx >= myWay.Count) 
                {
                    yield break; 
                }
                targetNode = myWay[idx];

                
            }
            
            //transform.position = Vector2.MoveTowards(transform.position, targetNode.myPos, Time.fixedDeltaTime);

            //test1.rigid.MovePosition(rigid.position+nextVec*Time.fixedDeltaTime);
            

            yield return null;
        }
    }

    /*int[,] ClosedIndex(Vector2 vector)
    {
        int x;
        int y;
        float halfW = coll.size.x/2;
        float halfH = coll.size.y/2;
        Layers layer;
        layer = Collision_Handler.instance.layer[0];
        Vector2[] points = new Vector2[layer.coll_count*4];
        for(int i=0;i<layer.coll_count;i++)
        {
            points[0].x = layer.layerPosMin[i].x + halfW;
            points[0].y = layer.layerPosMin[i].y + halfH;
            points[1].x = layer.layerPosMax[i].x + half
        }


            



    }*/
    Vector2 NextVec(Node my, Node target)
    {
        Vector2 nextVec;
        nextVec.x = target.myPos.x - my.myPos.x;
        nextVec.y = target.myPos.y - my.myPos.y;
        nextVec = new Vector2(nextVec.normalized.x, nextVec.normalized.y);
        return nextVec;
    }

}