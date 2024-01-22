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
    public bool canmove = true; //�ӽ�
    Vector2 inputvec;
    public float speed;
    public float jumpSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator animator;
    BoxCollider2D coll;
    public Vector2 horiVec;
    public Vector2 vertVec = Vector2.zero;
    public Vector2 horiAlpVec;
    public Vector2 vertAlpVec;
    public float[] playerPosX = new float[2];
    public float[] playerPosY = new float[2];
    public Vector2 ceil;
    public bool hitCeil = false;
    public bool hitVer = false;
    public string gravity = "falling";
    public bool jumpTrigger = false;
    public float jumpHeight;
    public int jumpCount;
    public bool spaceTrigger = true;
    public Vector2 wallVec;
    public bool wallTrigger = false;
    public int wallcount;
    public int wallmultiple;
    public int mapIndex = 0;


    void Awake()
    {
        instance = this;
        rigid = instance.player.GetComponent<Rigidbody2D>();
        animator = instance.player.GetComponent<Animator>();
        spriter = instance.player.GetComponent<SpriteRenderer>();
        coll = instance.player.GetComponent<BoxCollider2D>();
        horiAlpVec = Vector2.right * speed * Time.fixedDeltaTime;
        vertAlpVec = Vector2.up * jumpSpeed * Time.fixedDeltaTime;
    }



    private void LateUpdate()
    {
        animator.SetBool("ground", vertVec.y == 0);
        animator.SetFloat("speed", Mathf.Abs(inputvec.x));
        animator.SetFloat("vert", vertVec.y);
        if (jumpCount == 0 && vertVec.y > 0.01)
            animator.SetBool("doublejump", true);
        else
            animator.SetBool("doublejump", false);



        if (inputvec.x != 0)
        {
            spriter.flipX = inputvec.x < 0;
        }
        mapIndex = instance.player.currnetMap - 1;
    }



    public void GravityVec()
    {
        switch (gravity)
        {
            case "falling":
                vertVec = Vector2.down * jumpSpeed * Time.fixedDeltaTime;
                spaceTrigger = true;
                if (jumpTrigger)
                {
                    jumpCount--;
                    jumpTrigger = false;
                }
                break;

            case "jumping":
                if (spaceTrigger)
                    Jumping();
                break;
        }




    }

    public void MovingVec()
    {
        inputvec.x = Input.GetAxisRaw("Horizontal");
        if (canmove)
            horiVec = inputvec * speed * Time.fixedDeltaTime;
        wallVec = -inputvec * speed * Time.fixedDeltaTime * 2;
    }

    public void GetPlayerColl()
    {
        playerPosX[0] = rigid.position.x - coll.size.x / 2;
        playerPosY[0] = rigid.position.y - coll.size.y / 2;
        playerPosX[1] = rigid.position.x + coll.size.x / 2;
        playerPosY[1] = rigid.position.y + coll.size.y / 2;
    }

    List<string> IsPilePosition(Layers layer)
    {
        float curPosmin = 0;
        float curPosmax = 0;
        float mapPosmin = 0;
        float mapPosmax = 0;
        List<string> results = new List<string>();


        //x축먼저 검사
        curPosmin = playerPosX[0];
        curPosmax = playerPosX[1];
        for (int i = 0; i < layer.coll_count; i++)
        {
            mapPosmin = layer.layerPosMin[i].x;
            mapPosmax = layer.layerPosMax[i].x;
            if (mapPosmin < curPosmin && curPosmin < mapPosmax ||
                mapPosmin < curPosmax && curPosmax < mapPosmax ||
                curPosmin >= mapPosmin && mapPosmax <= curPosmax)
            {
                results.Add(IsPileNextPosition(layer, i, 'y'));
            }

        }
        //y축검사
        curPosmin = playerPosY[0];
        curPosmax = playerPosY[1];
        for (int i = 0; i < layer.coll_count; i++)
        {
            mapPosmin = layer.layerPosMin[i].y;
            mapPosmax = layer.layerPosMax[i].y;
            if (mapPosmin < curPosmin && curPosmin < mapPosmax ||
                mapPosmin < curPosmax && curPosmax < mapPosmax ||
                curPosmin >= mapPosmin && mapPosmax <= curPosmax)
            {
                results.Add(IsPileNextPosition(layer, i, 'x'));
            }
        }


        return results;
    }

    string IsPileNextPosition(Layers layer, int i, char dir)
    {
        float next = 0;
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
                curPosmax = playerPosY[1];
                curPosmin = playerPosY[0];
                next = vertAlpVec.y;
                if (mapPosmax > curPosmax + next &&
                    curPosmax + next > mapPosmin)
                    result = "up";
                
                if (mapPosmax > curPosmin - next &&
                    curPosmin - next > mapPosmin)
                    result = "down";
            break;

            case ('x') : 
                mapPosmax = layer.layerPosMax[i].x;
                mapPosmin = layer.layerPosMin[i].x;
                curPosmax = playerPosX[1];
                curPosmin = playerPosX[0];

                if (mapPosmax > curPosmax + next &&
                    curPosmax + next > mapPosmin)
                    result = "left";

                if (mapPosmax < curPosmin - next &&
                    curPosmin - next > mapPosmin)
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
        
        
        foreach(string dir in collisionDir)
        {
            Debug.Log(dir);
            switch(dir)
            {
                case ("down") :
                vertVec.y = Mathf.Clamp(vertVec.y, 0, 1);
                jumpCount = 2;
                spaceTrigger = true;
                jumpTrigger = true;
                break;

                case("up") :
                vertVec.y = Mathf.Clamp(vertVec.y, -1, 0);
                gravity = "falling";
                break;

                case("left") :
                if(Input.GetAxisRaw("Horizontal") == -1)
                {
                    horiVec.x = Mathf.Clamp(horiVec.x, 0, 1);
                    wallcount++;
                }
                break;

                case("right") :
                if(Input.GetAxisRaw("Horizontal") == 1)
                {
                    horiVec.x = Mathf.Clamp(horiVec.x, -1, 0);
                    wallcount++;
                }
                break;

            }
        }

        wallTrigger = wallcount > 0;
    }

    public void Jumping()
    {
        spaceTrigger = false;
        if (wallTrigger && jumpCount == 1)
        {
            canmove = false;
            horiVec += wallVec;
            animator.SetBool("walljump", true);
            StartCoroutine(delay());
        }
        if (jumpCount > 0)
        {
            if (jumpTrigger)
                jumpTrigger = false;
            SetCeil();
            vertVec = Vector2.up * jumpSpeed * Time.fixedDeltaTime;
            jumpCount--;

        }
    }


    void SetCeil()
    {
        ceil.y = playerPosX[1] + jumpHeight;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(0.3f);
        canmove = true;
        animator.SetBool("walljump", false);
    }


}
