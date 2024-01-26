using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Monster : MonoBehaviour
{
    Rigidbody2D rigid;
    BoxCollider2D coll;
    Animator animator;
    public float[] monsterPosX = new float[2];
    public float[] monsterPosY = new float[2];
    public float mharfLef;
    public float mharfRig;
    public Layers[] layers = new Layers[4];
    bool move = true;
    const int hpint = 2;
    public int hp;

    void OnEnable()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        for(int i=0;i<Collision_Handler.instance.layer.Length;i++)
            layers[i] = Collision_Handler.instance.layer[i];
        for(int i=0; i<2; i++)
        {
            monsterPosX[i] = 0;
            monsterPosY[i] = 0;
        }
        Init();
        
    }

    void FixedUpdate()
    {
        
        GetmonsterColl();
        CollisionCheck();
        if(move)
        {
            rigid.MovePosition(rigid.position+Vector2.down*Time.fixedDeltaTime * 2);
            animator.SetFloat("vert", -0.1f);
        }
        if(!move)
        {
            animator.SetBool("ground", true);
            animator.SetFloat("vert", 0);
        }

    }

    void LateUpdate()
    {

        CheckHP();
    }

    public void Init()
    {
        hp = hpint;
        move = true;
        rigid.transform.position = new Vector3(Collision_Handler.instance.player.playerPosX[0], -0.16f, 0);
        
    }

    void CheckHP()
    {
        if(hp==0)
        {
            animator.SetTrigger("Hit");
            StartCoroutine(Deadwait());
            Collision_Handler.instance.pool.eachmobs.Remove(this);
        }
    }
    void CollisionCheck()
    {
        Layers layer = layers[Collision_Handler.instance.mapIndex];
        for(int i=0;i<layer.coll_count;i++)
        {
            if(layer.layerPosMin[i].x < monsterPosX[0] && monsterPosX[0] < layer.layerPosMax[i].x ||
               layer.layerPosMin[i].x < monsterPosX[1] && monsterPosX[1] < layer.layerPosMax[i].x ||
               layer.layerPosMin[i].x >= monsterPosX[0] && monsterPosX[0] >= layer.layerPosMax[i].x)
               {
                    if(monsterPosY[0] - Collision_Handler.instance.vertNext < layer.layerPosMax[i].y)
                        move = false;

               }
        }
    }
    public void GetmonsterColl()
    {
        monsterPosX[0] = rigid.position.x - coll.size.x / 2;
        monsterPosY[0] = rigid.position.y - coll.size.y / 2;
        monsterPosX[1] = rigid.position.x + coll.size.x / 2;
        monsterPosY[1] = rigid.position.y + coll.size.y / 2;
        mharfLef = rigid.position.x - coll.size.x / 4;
        mharfRig = rigid.position.x + coll.size.x / 4;
    }

    IEnumerator Deadwait()
    {
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
    }
}
