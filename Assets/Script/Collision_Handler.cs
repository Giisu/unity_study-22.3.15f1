using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collision_Handler : MonoBehaviour
{
    public static Collision_Handler instance;
    public Player player;
    public Layers layer;
    public ContactPoint2D contacts;
    public bool canmove = true; //임시
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
    public Vector2 playerPosMin;
    public Vector2 playerPosMax;
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

    private void FixedUpdate()
    {
        

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
                if(spaceTrigger)
                    Jumping();
                break;
        }



    }

    public void MovingVec()
    {
        inputvec.x = Input.GetAxisRaw("Horizontal");
        if(canmove)
            horiVec = inputvec * speed * Time.fixedDeltaTime;
        wallVec = -inputvec * speed * Time.fixedDeltaTime * 2;
    }

    public void GetPlayerColl()
    {
        playerPosMin.x = rigid.position.x - coll.size.x / 2;
        playerPosMin.y = rigid.position.y - coll.size.y / 2;
        playerPosMax.x = rigid.position.x + coll.size.x / 2;
        playerPosMax.y = rigid.position.y + coll.size.y / 2;
    }

    public void CollisionCheck()
    {
        wallcount = 0;
        for (int i = 0; i < layer.coll_count; i++)
        //캐릭터 x포지션이 콜라이더 x포지션과 겹쳐있는 상태에서
        {
            if (layer.layerPosMin[i].x < playerPosMin.x && playerPosMin.x < layer.layerPosMax[i].x
                || layer.layerPosMin[i].x < playerPosMax.x && playerPosMax.x < layer.layerPosMax[i].x)
            {
                //y축까지 겹치면(바닥면과)
                if (layer.layerPosMax[i].y > playerPosMin.y - vertAlpVec.y && playerPosMin.y - vertAlpVec.y > layer.layerPosMin[i].y)
                {
                    vertVec.y = Mathf.Clamp(vertVec.y, 0, 1);//수직이동 제한
                    jumpCount = 2;
                    spaceTrigger = true;
                    jumpTrigger = true;
                }
                //y축 천장면과 겹치면
                if (layer.layerPosMax[i].y > playerPosMax.y + vertAlpVec.y && playerPosMax.y + vertAlpVec.y > layer.layerPosMin[i].y
                    || playerPosMax.y + vertAlpVec.y > ceil.y)
                {
                    vertVec.y = Mathf.Clamp(vertVec.y, -1, 0);//수직이동 제한
                    gravity = "falling";
                }
            }
            //캐릭터 y포지션이 콜라이더 y포지션과 겹쳐있는 상태에서

            if (layer.layerPosMin[i].y < playerPosMin.y && playerPosMin.y < layer.layerPosMax[i].y
                || layer.layerPosMin[i].y < playerPosMax.y && playerPosMax.y < layer.layerPosMax[i].y)
            {
                //x축까지 겹치면
                if (layer.layerPosMin[i].x < playerPosMin.x - horiAlpVec.x && playerPosMin.x - horiAlpVec.x < layer.layerPosMax[i].x
                    && Input.GetAxisRaw("Horizontal") == -1)
                {
                    horiVec.x = Mathf.Clamp(horiVec.x, 0, 1);
                    wallcount++;


                }

                if (layer.layerPosMin[i].x < playerPosMax.x + horiAlpVec.x && playerPosMax.x + horiAlpVec.x < layer.layerPosMax[i].x
                && Input.GetAxisRaw("Horizontal") == 1)
                {
                    horiVec.x = Mathf.Clamp(horiVec.x, -1, 0);
                    wallcount++;

                }



            }
        }
        wallTrigger = wallcount > 0;
    }

    public void Jumping()
    {
        spaceTrigger = false;
        if(wallTrigger && jumpCount==1)
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
        ceil.y = playerPosMax.y + jumpHeight;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(0.3f);
        canmove = true;
        animator.SetBool("walljump", false);
    }
}
