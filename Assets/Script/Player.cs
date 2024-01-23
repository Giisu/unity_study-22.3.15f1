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

    public float[] playerPosX = new float[2];
    public float[] playerPosY = new float[2];
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
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
        GetPlayerColl();

        space = Collision_Handler.instance.spaceTrigger == true? Input.GetKey(KeyCode.Space) : false;
        if (space)
            Collision_Handler.instance.Jumping(false);

        
        MovingVec();
        Collision_Handler.instance.Falling();
        Collision_Handler.instance.CollisionCheck();
        Collision_Handler.instance.MonsterCollisionCheck();
        CheckTransfer();
        Repos();

    }



    public void Repos()
    {
        rigid.MovePosition(rigid.position + horiVec + vertVec);
    }

    public void CheckTransfer()
    {
        for(int i=0;i<transfer.data.Length;i++)
        {
            string side = transfer.data[i].whichSide;
            int mapNum = transfer.data[i].mapNum;
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
