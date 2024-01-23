using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Rigidbody2D rigid;
    BoxCollider2D coll;
    public float[] monsterPosX = new float[2];
    public float[] monsterPosY = new float[2];

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        for(int i=0; i<2; i++)
        {
            monsterPosX[i] = 0;
            monsterPosY[i] = 0;
        }
    }

    void FixedUpdate()
    {
        GetmonsterColl();
    }

    public void GetmonsterColl()
    {
        monsterPosX[0] = rigid.position.x - coll.size.x / 2;
        monsterPosY[0] = rigid.position.y - coll.size.y / 2;
        monsterPosX[1] = rigid.position.x + coll.size.x / 2;
        monsterPosY[1] = rigid.position.y + coll.size.y / 2;
    }
}
