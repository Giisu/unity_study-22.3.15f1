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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && Collision_Handler.instance.spaceTrigger)
            Collision_Handler.instance.gravity = "jumping";
 
        Collision_Handler.instance.MovingVec();
        Collision_Handler.instance.GravityVec();
        Collision_Handler.instance.GetPlayerColl();
        Collision_Handler.instance.CollisionCheck();
        Repos();
        CheckTransfer();

    }

    public void Repos()
    {
        rigid.MovePosition(rigid.position + Collision_Handler.instance.horiVec + Collision_Handler.instance.vertVec);
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

}
