using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Collision_Handler : MonoBehaviour
{
    public static Collision_Handler instance;
    public Transfer transfer;
    public Player player;
    public Layers[] layer;
    public ContactPoint2D contacts;
    public List<Monster> monster;
    HUD hud;

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
    public bool islive = false;
    
    
    
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator animator;
    BoxCollider2D coll;
    public PoolManager pool;
    
    public Vector2 ceil;
    
    public bool spaceTrigger = true;
  
    public bool wallTrigger = false;
    public int wallcount;
    public int mapIndex = 0;
    public bool isjump = false;
    bool iscoll = false;
    float pPos = 0;
    public int playerId;
    


    void Awake()
    {
        instance = this;
        rigid = instance.player.GetComponent<Rigidbody2D>();
        animator = instance.player.GetComponent<Animator>();
        
        spriter = instance.player.GetComponent<SpriteRenderer>();
        vertNext = Vector2.up.y * player.jumpSpeed * Time.fixedDeltaTime;
        horiNext = Vector2.right.x * player.speed * Time.fixedDeltaTime;
        //블럭충돌 계산을 위해 플레이어가 1 fixed frame당 이동하는 거리를 미리 선언
        hud = FindObjectOfType<HUD>();
        Stop();    //캐릭터 선택을 위해 우선 게임 정지
        

        
    }
    public void Gamestart(int id) //해당 함수는 HUD이미지의 onclick 이벤트를 통해 실행되고 매개변수를 받아옴
    {
        playerId = id;    //player스크립트는 onenable시 playerid를 받아서 해당 animator를 재생. 
        
        player.gameObject.SetActive(true); //플레이어 오브젝트는 처음에 비활성화해둠. 캐릭터가 변경될 수 있기때문
        Resume(); //게임재개
    }

    public void Stop()
    {
        islive = false;
        Time.timeScale = 0;
    }

    public void Gameover()
    {
        animator.SetTrigger("Disappear");   
        islive = false;
        StartCoroutine(GameoverRoutine());

    }

    IEnumerator GameoverRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
        
    }

    public void Resume()
    {
        islive = true;
        Time.timeScale = 1;
    }


    private void LateUpdate()
    {
        if(!islive)
            return;
        animator.SetBool("ground", player.vertVec.y == 0); //애니메이터 파라미터. 수직벡터 없으면 ground true
        animator.SetFloat("speed", Mathf.Abs(player.inputvec.x));//speed는 0.1 이상이면 run 애니메이션 실행
        animator.SetFloat("vert", player.vertVec.y); //vert는 값에따라 점프, 추락 애니메이션
        if (player.jumpCount == 0 && player.vertVec.y > 0.01) //두번째 점프를 실행했고, 아직 점프중이라면
            animator.SetBool("doublejump", true);  
        else
            animator.SetBool("doublejump", false);

        if (player.inputvec.x != 0)     
        {
            spriter.flipX = player.inputvec.x < 0; //flip은 spriter에 들어있음.
        }
        mapIndex = instance.player.currnetMap - 1; //어떤 index의 레이어를 불러와야 하는지, 카메라가 어디를 비춰야하는지를 확인
    }

    public void MonsterCollisionCheck()
    {
        foreach(Monster monster in pool.eachmobs)
        //pool manager에서 포지션만 따로 담아두었던 이유
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

        if (monster.mharfLef < pPos && pPos < monster.mharfRig) //적 밟은 판정의 범위는 블럭보다 좀 좁게
        {
            if (mTop > pBot-vertNext && mBot < pBot-vertNext)//적 밟음
            {
                Collision_Handler.instance.Jumping(true);//jumping은 매개변수에 따라 2개의 동작
                monster.hp--;          //몬스터 hp 차감
            }
            
        }

        else if (mBot < pTop && pTop < mTop ||
                 mBot < pBot && pBot < mTop ||
                 pBot <= mBot && mTop <= pTop)
        // 몬스터 밟음 판정을 제외하고 나머지는 크게 분류하지 않고, 왼쪽 오른쪽만으로 구분함.
        {
            if (mRig > pLef-horiNext && pLef-horiNext > mLef)
                Hit("right");
            if(mLef < pRig+horiNext && pRig+horiNext < mRig)
                Hit("left");
        }
        }

    }
    void Hit(string dir)
    {
        player.canmove = false; //피격 판정 도중에는 움직일 수 없음.
        isjump = false;
        //falling, 즉 중력효과는 isjump시에 동작하지 않고 return. 
        if(!animator.GetBool("Hit"))
            hud.index--;
            //hud의 index는 hp칸의 현재 개수를 나타냄.
            //또한 다중피격을 방지하기 위해 Hit애니메이션이 재생중인지 확인하고 재생중이라면 감소시키지 않음
        
        animator.SetBool("Hit", true);
        player.vertVec.y = Mathf.Clamp(player.vertVec.y,-1,0);  //y는 음수일 수 있지만 양수일 수 없음.
        switch(dir)
        {
            case ("right") :
            player.horiVec.x = 1f * Time.fixedDeltaTime;
            break;

            case("left") :
            player.horiVec.x = -0.3f * Time.fixedDeltaTime;
            break;

        }
        
        player.rigid.MovePosition(player.rigid.position + player.horiVec + player.vertVec);
        //정리하자면 피격시 몬스터 충돌방향과 반대로 튕겨나가고, 점프중이라면 점프는 취소, 추락은 유지.

        
        StartCoroutine(Canmove_after_hit_delay());
        //코루틴을 통해 0.25초 딜레이를 주고, 이후 canmove는 다시 true. hit 애니메이션 false
    }

    void FixedUpdate()
    { 
        if(hud.index == 0)
            Gameover();
    }

    IEnumerator Canmove_after_hit_delay()
    {
        yield return new WaitForSeconds(0.25f);
        player.canmove = true;
        animator.SetBool("Hit",false);
    }
    public void Jumping(bool hit)
    {
        
        spaceTrigger = false;
        //space trigger는 점프 중복입력을 방지하기 위한 이중장치. 
        //player 스크립트에서, space trigger여야만 bool space에 입력을 받을 수 있고, 다시 space가 true일 경우 해당
        //함수가 실행됨.
        
        if(hit) //몬스터를 밟음으로서 jumping이 실행된 경우라면
        {
            ceil.y = player.playerPosY[1] + player.jumpHeight*0.7f; //jump height보다 조금 낮은 높이로 점프함.
            player.vertVec = Vector2.up * player.jumpSpeed * Time.fixedDeltaTime; //점프 속도자체는 동일
            player.jumpCount = 1;// 더블점프, 월점프가 가능
            isjump = true; //중력효과는 당연히 적용하지 않아야함.
            return;
        }

        if(wallTrigger && player.jumpCount == 1 && Input.GetKey(KeyCode.Space) && !isjump && player.canmove)
        //walltrigger는 좌/우 벽에 닿아있을 경우 true. 또한 점프카운트가 1회여야만 함. 바닥에서 뛸 때는 적용x
        //space를 누르지 않았는데 벽점프가 실행되는 경우를 방지하기 위해 space키 역시 받아옴. 
        //isjump는 천장에 닿거나 최대높이에 닿았을 때 
        //캔무브는 왜 넣었는지 기억안남
        {
            player.canmove = false; //피격과 마찬가지로 조작은 잠시 안되게
            player.horiVec += player.revHoriVec; //방향키 입력시 입력방향, 역방향 두 벡터를 받아오고 여기서는 역방향 벡터를 사용
            animator.SetBool("walljump", true);
            StartCoroutine(Canmove_afterdelay());
            animator.SetBool("walljump", false);
        }
        if (player.jumpCount > 0 && player.canmove) //일반적인 점프의 경우.
        {
            SetCeil(); //천장높이는 플레이어 현재 top포지션 + jumpheight.
            player.vertVec = Vector2.up * player.jumpSpeed * Time.fixedDeltaTime;
            player.jumpCount--;
        }
        isjump = true;
    }

    public void Falling()
    {
        if(isjump || !player.canmove) //점프중, canmove false상태면 중력효과 x
            return;
        player.vertVec = Vector2.down * player.jumpSpeed * Time.fixedDeltaTime;
    }

    

    

    List<string> IsPilePosition(Layers layer) 
    //string list를 반환하는 함수이며 map index에 따른 layers를 받아옴
    {
        float curPosmin = 0;
        float curPosmax = 0;
        float mapPosmin = 0;
        float mapPosmax = 0;
        List<string> results = new List<string>(); //여러 방향의 충돌이 있을 수 있기에 list도 만들어줌
        string result;

        //x축먼저 검사
        curPosmin = player.playerPosX[0];
        curPosmax = player.playerPosX[1];
        for (int i = 0; i < layer.coll_count; i++) //coll_count는 layers배열 각각의 length를 담고있음.
        {
            mapPosmin = layer.layerPosMin[i].x;
            mapPosmax = layer.layerPosMax[i].x;
            if (mapPosmin < curPosmin && curPosmin < mapPosmax || 
                mapPosmin < curPosmax && curPosmax < mapPosmax ||
                //블럭 x범위(예컨데 바닥면)과 플레이어 좌, 우 끝점이 겹치는지 각각 확인
                curPosmin <= mapPosmin && mapPosmax <= curPosmax)
                //플레이어가 블럭보다 큰 경우, 오히려 블럭이 플레이어 x범위에 들어오는지 확인(이 경우 양 모서리 모두가 들어오는것이 됨)
            {
                result = IsPileNextPosition(layer, i, 'y');
                //해당 함수는 layer, layer의 인덱스값 i, 어느 방향을 검사할지 지정하는 string dir을 매개변수로 함.
                //이 경우 추가로 y축을 검사해야하는 것이기 때문에, y가 됨.
                if(result != null)
                    results.Add(result);//겹치는것을 result에 추가.
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
                //전부 똑같지만 y를 먼저 검사
            {
                result = IsPileNextPosition(layer, i, 'x');
                //x를 추가검사.
                if(result != null)
                    results.Add(result);
            }
            
        }
        if(curPosmax >= ceil.y) //점프 최대높이에 닿는 경우도 사실상 천장에 닿는것과 완전히 같기에 별도의 조건문을 추가해줌.
            results.Add("up"); 
        return results;//이후 모든 dir을 return해줌
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
                //비교 자체는 위와 비슷한데, 현재 위치를 기준으로 하면 포지션이 이미 이동된 후이기에 
                //겹침 상태가 계속 유지되게됨. 따라서 포지션+이동거리만큼으로 조건문을 계산하게됨.
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
        wallcount = 0; //wall trigger를 활성화하기 위해
        List<string> collisionDir = new List<string>(); 
        collisionDir = IsPilePosition(layer[mapIndex]);
        int dircount=0; //점프하지 않고 떨어졌을때를 판단하기 위함
        
        foreach(string dir in collisionDir) //위에서 충돌체크 함수를 통해 받아온 dir list를 하나하나 검사
        {
            switch(dir)
            {
                case ("down") :
                player.vertVec.y = Mathf.Clamp(player.vertVec.y, 0, 1);//바닥에 닿은상태면 수직벡터는 음수 불가능
                player.jumpCount = 2; //점프카운트 초기화
                isjump = false;  
                dircount++; 
                spaceTrigger = true; //점프 가능
                break;

                case("up") :
                player.vertVec.y = Mathf.Clamp(player.vertVec.y, -1, 0); //천장에 닿으면 위로 올라갈 수 없음ㄷ
                isjump = false; //중력효과 재적용
                dircount++;
                if(!spaceTrigger)    //더블점프, 벽점프는 아래 코루틴을 통해 delay를 기다린 후에 실행할 수 있음.
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
        if(dircount == 0 && player.jumpCount == 2) //점프하지 않고 떨어졌을때
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
