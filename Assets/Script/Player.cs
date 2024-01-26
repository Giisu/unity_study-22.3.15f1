using System.Collections;
using System.Runtime.CompilerServices;
using System.Transactions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public int currnetMap = 1;
    public Rigidbody2D rigid;
    public Transfer transfer;
    BoxCollider2D coll;
    public bool space = false;
    public Vector2 inputvec;
    public bool canmove = true; 
    public Vector2 horiVec;
    public Vector2 vertVec = Vector2.zero;
    public Vector2 horiAlpVec;
    public Vector2 vertAlpVec;
    public float speed;
    public float jumpSpeed;
    public float jumpHeight;
    public int jumpCount;
    public Vector2 revHoriVec;
    public Vector2 revVertVec;
    Animator animator;
    public float[] playerPosX = new float[2];
    public float[] playerPosY = new float[2];
    public RuntimeAnimatorController[] animCon;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        // playerPosX = new float[2];
        // playerPosY = new float[2];
        for(int i=0; i<2; i++)
        {
            playerPosX[i] = 0;
            playerPosY[i] = 0;
        }
    }
    private void FixedUpdate()
    {   
        if(!Collision_Handler.instance.islive)
            return;
        GetPlayerColl();

        space = Collision_Handler.instance.spaceTrigger == true? Input.GetKey(KeyCode.Space) : false;
        if (space)
            Collision_Handler.instance.Jumping(false);

        
        MovingVec();                                       //좌/우 방향키를 통해 벡터값을 받아오고
        Collision_Handler.instance.Falling();              //isjump 여부, 점프 여부에 따라 y벡터값을 받아온 후
        Collision_Handler.instance.CollisionCheck();       //충돌체크를 통해 벡터값 보정이 필요한지 판단하고
        Collision_Handler.instance.MonsterCollisionCheck(); //몬스터 충돌체크를 통해 hit, 벡터 보정이 필요한지 판단하고
        Repos();                                            //최종 벡터값에 따라 위치를 재조정하며
        CheckTransfer();                                    //이동한 포지션에 맵 이동 포인트가 있는지를 판단한다.
        

    }
    void OnEnable()
    {
        animator.runtimeAnimatorController = animCon[Collision_Handler.instance.playerId];
        animator.SetTrigger("Appear");
        //활성화시(생성시) 플레이어 ID를 받아 애니메이터를 지정해주고, appear 애니메이션 재생
    }


    public void Repos()
    {
        rigid.MovePosition(rigid.position + horiVec + vertVec);
    }

    public void CheckTransfer()
    {
        for(int i=0;i<transfer.data.Length;i++)
        //모든 transfer point를 전부 검사. 나중에 foreach로 바꾸면 더 좋을듯
        {
            string side = transfer.data[i].whichSide;   //맵의 어느방향에 있는 포인트인지
            int mapNum = transfer.data[i].mapNum;       //현재 맵의 넘버가 어떻게 되는지
            Vector3 currentPos = rigid.position;       
            Vector3 minPos = transfer.data[i].minPos;  
            Vector3 maxPos = transfer.data[i].maxPos;
            float middleX = (transfer.data[i].minPos.x + transfer.data[i].maxPos.x)/2;

            switch(side) 
            {  
                case "left" :
                    if(minPos.x < currentPos.x && currentPos.x < maxPos.x && currentPos.x < middleX)
                    {
                        int index = i;
                        currnetMap -= 1;
                        if(currnetMap < 1)
                        {
                            currnetMap = 4;
                            index = 8;
                        }
                        Vector3 nextPos = (transfer.data[index-1].minPos + transfer.data[index-1].maxPos)/2;
                        nextPos.y = 0;
                        currentPos.x = 0;
                        rigid.MovePosition(transfer.data[index-1].currentPos);
                    }
                break;

                case "right" :
                    if(minPos.x < currentPos.x && currentPos.x < maxPos.x && currentPos.x > middleX)
                    {
                        int index = i;
                        currnetMap += 1;
                        if(currnetMap > 4)
                        {
                            currnetMap = 1;
                            index=-1;
                        }
                        Vector3 nextPos = (transfer.data[index+1].minPos + transfer.data[index+1].maxPos)/2;
                        nextPos.y = 0;
                        currentPos.x = 0;
                        rigid.MovePosition(transfer.data[index+1].currentPos);
                    }
                break;

                



            }

        }
    }

    public void GetPlayerColl()
    {
        playerPosX[0] = rigid.position.x - coll.size.x / 2;
        playerPosY[0] = rigid.position.y - coll.size.y / 2;
        playerPosX[1] = rigid.position.x + coll.size.x / 2;
        playerPosY[1] = rigid.position.y + coll.size.y / 2;
    }

    public void MovingVec()
    {
        inputvec.x = Input.GetAxisRaw("Horizontal");
        if (canmove)
            horiVec = inputvec * speed * Time.fixedDeltaTime;
        revHoriVec = -inputvec * speed * Time.fixedDeltaTime * 2;
    }

   
    

    
}
