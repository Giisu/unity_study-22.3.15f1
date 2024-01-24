using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collision_Handler : MonoBehaviour
{
    public static Collision_Handler instance;
    public Transfer transfer;
    public Player player;
    public Layers[] layer;
    public ContactPoint2D contacts;
    public Monster monster;

    float pTop;
    float pBot;
    float pLef;
    float pRig;
    float mTop;
    float mBot;
    float mLef;
    float mRig;
    public float horiNext;
    public float vertNext;
    
    
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator animator;
    BoxCollider2D coll;
    
    
    public Vector2 ceil;
    
    public bool spaceTrigger = true;
  
    public bool wallTrigger = false;
    public int wallcount;
    public int mapIndex = 0;
    public bool isjump = false;
    bool iscoll = false;
    float pPos = 0;


    void Awake()
    {
        instance = this;
        rigid = instance.player.GetComponent<Rigidbody2D>();
        animator = instance.player.GetComponent<Animator>();
        spriter = instance.player.GetComponent<SpriteRenderer>();
        vertNext = Vector2.up.y * player.jumpSpeed * Time.fixedDeltaTime;
        horiNext = Vector2.right.x * player.speed * Time.fixedDeltaTime;

        
    }



    private void LateUpdate()
    {
        animator.SetBool("ground", player.vertVec.y == 0);
        animator.SetFloat("speed", Mathf.Abs(player.inputvec.x));
        animator.SetFloat("vert", player.vertVec.y);
        if (player.jumpCount == 0 && player.vertVec.y > 0.01)
            animator.SetBool("doublejump", true);
        else
            animator.SetBool("doublejump", false);

        if (player.inputvec.x != 0)
        {
            spriter.flipX = player.inputvec.x < 0;
        }
        mapIndex = instance.player.currnetMap - 1;
    }

    public void MonsterCollisionCheck()
    {
        
        pTop = player.playerPosY[1]; //플레이어 상하좌우 포지션
        pBot = player.playerPosY[0];
        pLef = player.playerPosX[0];
        pRig = player.playerPosX[1];

        mTop = monster.monsterPosY[1]; //몬스터 상하좌우 포지션
        mBot = monster.monsterPosY[0];
        mLef = monster.monsterPosX[0];
        mRig = monster.monsterPosX[1];

        pPos = player.rigid.position.x; //플레이어 센터포지션 x

        if (monster.mharfLef < pPos && pPos < monster.mharfRig)
        {
            if (mTop > pBot-vertNext && mBot < pBot-vertNext)//적 밟음
            {
                Collision_Handler.instance.Jumping(true);
            }
            
        }

        else if (mBot < pTop && pTop < mTop ||
                 mBot < pBot && pBot < mTop ||
                 pBot <= mBot && mTop <= pTop)
        {
            if (mRig > pLef-horiNext && pLef-horiNext > mLef)
                Hit("right");
            if(mLef < pRig+horiNext && pRig+horiNext < mRig)
                Hit("left");
        }


    }
    void Hit(string dir)
    {
        player.canmove = false;
        player.vertVec.y = -0.3f * Time.fixedDeltaTime;
        switch(dir)
        {
            case ("right") :
            player.horiVec.x = 1f * Time.fixedDeltaTime;
            Debug.Log("monsterright");
            break;

            case("left") :
            player.horiVec.x = -0.3f * Time.fixedDeltaTime;
            Debug.Log("monsterleft");
            break;

        }
        
        player.rigid.MovePosition(player.rigid.position + player.horiVec + player.vertVec);
        
        StartCoroutine(Canmove_after_hit_delay());
    }

    IEnumerator Canmove_after_hit_delay()
    {
        yield return new WaitForSeconds(0.2f);
        player.canmove = true;
    }
    public void Jumping(bool hit)
    {
        
        spaceTrigger = false;
        
        if(hit)
        {
            ceil.y = player.playerPosY[1] + player.jumpHeight*0.7f;
            player.vertVec = Vector2.up * player.jumpSpeed * Time.fixedDeltaTime;
            player.jumpCount = 1;
            isjump = true;
            return;
        }

        if(wallTrigger && player.jumpCount == 1 && Input.GetKey(KeyCode.Space) && !isjump && player.canmove)
        {
            player.canmove = false;
            player.horiVec += player.revHoriVec;
            animator.SetBool("walljump", true);
            StartCoroutine(Canmove_afterdelay());
            animator.SetBool("walljump", false);
        }
        if (player.jumpCount > 0 && player.canmove)
        {
            SetCeil();
            player.vertVec = Vector2.up * player.jumpSpeed * Time.fixedDeltaTime;
            player.jumpCount--;
        }
        isjump = true;
    }

    public void Falling()
    {
        if(isjump && !player.canmove)
            return;
        player.vertVec = Vector2.down * player.jumpSpeed * Time.fixedDeltaTime;
    }

    

    

    List<string> IsPilePosition(Layers layer)
    {
        float curPosmin = 0;
        float curPosmax = 0;
        float mapPosmin = 0;
        float mapPosmax = 0;
        List<string> results = new List<string>();
        string result;

        //x축먼저 검사
        curPosmin = player.playerPosX[0];
        curPosmax = player.playerPosX[1];
        for (int i = 0; i < layer.coll_count; i++)
        {
            mapPosmin = layer.layerPosMin[i].x;
            mapPosmax = layer.layerPosMax[i].x;
            if (mapPosmin < curPosmin && curPosmin < mapPosmax ||
                mapPosmin < curPosmax && curPosmax < mapPosmax ||
                curPosmin <= mapPosmin && mapPosmax <= curPosmax)
            {
                result = IsPileNextPosition(layer, i, 'y');
                if(result != null)
                    results.Add(result);
            }

        }
        //y축검사
        curPosmin = player.playerPosY[0];
        curPosmax = player.playerPosY[1];
        for (int i = 0; i < layer.coll_count; i++)
        {
            mapPosmin = layer.layerPosMin[i].y;
            mapPosmax = layer.layerPosMax[i].y;
            if (mapPosmin < curPosmin && curPosmin < mapPosmax ||
                mapPosmin < curPosmax && curPosmax < mapPosmax ||
                curPosmin <= mapPosmin && mapPosmax <= curPosmax)
            {
                result = IsPileNextPosition(layer, i, 'x');
                if(result != null)
                    results.Add(result);
            }
            
        }
        if(curPosmax >= ceil.y) //점프 최대높이
            results.Add("up");
        return results;
    }

    string IsPileNextPosition(Layers layer, int i, char dir)
    {
        string result = null;
        float mapPosmax;
        float mapPosmin;
        float curPosmax;
        float curPosmin;

        switch (dir)
        {
            case ('y') :
                mapPosmax = layer.layerPosMax[i].y;
                mapPosmin = layer.layerPosMin[i].y;
                curPosmax = player.playerPosY[1];
                curPosmin = player.playerPosY[0];
                
                if (mapPosmax > curPosmax + vertNext &&
                    curPosmax + vertNext > mapPosmin)
                    result = "up";
                
                if (mapPosmax > curPosmin - vertNext &&
                    curPosmin - vertNext > mapPosmin)
                    result = "down";
            break;

            case ('x') : 
                mapPosmax = layer.layerPosMax[i].x;
                mapPosmin = layer.layerPosMin[i].x;
                curPosmax = player.playerPosX[1];
                curPosmin = player.playerPosX[0];

                if (mapPosmax >= curPosmin - horiNext &&
                    curPosmin - horiNext > mapPosmin)
                    result = "left";

                if (mapPosmin <= curPosmax + horiNext &&
                    curPosmax + horiNext < mapPosmax)
                    result = "right";
                break;
        }
        return result;
    }

    public void CollisionCheck()
    {
        wallcount = 0;
        List<string> collisionDir = new List<string>();
        collisionDir = IsPilePosition(layer[mapIndex]);
        int dircount=0;
        
        foreach(string dir in collisionDir)
        {
            Debug.Log(dir);
            switch(dir)
            {
                case ("down") :
                player.vertVec.y = Mathf.Clamp(player.vertVec.y, 0, 1);
                player.jumpCount = 2;
                isjump = false;
                dircount++;
                spaceTrigger = true;
                break;

                case("up") :
                player.vertVec.y = Mathf.Clamp(player.vertVec.y, -1, 0);
                isjump = false;
                dircount++;
                if(!spaceTrigger)
                    StartCoroutine(SpaceT_active_afterdelay());
                break;

                case("left") :
                player.horiVec.x = Mathf.Clamp(player.horiVec.x, 0, 1);
                wallcount++;
                dircount++;
                break;

                case("right") :
                player.horiVec.x = Mathf.Clamp(player.horiVec.x, -1, 0);
                wallcount++;
                dircount++;
                break;
            }
        }
        if(dircount == 0 && player.jumpCount == 2)
            player.jumpCount=1;
        wallTrigger = wallcount > 0;
    }

    

    void SetCeil()
    {
        ceil.y = player.playerPosY[1] + player.jumpHeight;
    }

    IEnumerator SpaceT_active_afterdelay()
    {
        yield return new WaitForSeconds(0.15f);
        spaceTrigger = true;
    }

    IEnumerator Canmove_afterdelay()
    {
        yield return new WaitForSeconds(0.3f);
        player.canmove = true;
    }


}
