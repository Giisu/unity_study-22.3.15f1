using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterCollision : MonoBehaviour
{
    public Player player;
    public Monster monster;

    float pTop;
    float pBot;
    float pLef;
    float pRig;
    float mTop;
    float mBot;
    float mLef;
    float mRig;



    void MonsterCollisionCheck()
    {
        pTop = player.playerPosY[1];
        pBot = player.playerPosY[0];
        pLef = player.playerPosX[0];
        pRig = player.playerPosX[1];
        mTop = monster.monsterPosY[1];
        mBot = monster.monsterPosY[0];
        mLef = monster.monsterPosX[0];
        mRig = monster.monsterPosX[1];
        if (mLef < pLef && pLef < mRig ||
            mLef < pRig && pRig < mRig ||
            pLef <= mLef && mRig <= pRig)
        {
            if (mTop >= pBot && mBot < pBot)//적 밟음
            {
                Collision_Handler.instance.Jumping(true);
                return;
            }
            if (pTop >= mBot && pBot < mBot)//적에게 밟힘
            {
                Hit();
                return;
            }
        }

        if (mBot < pTop && pTop < mTop ||
           mBot < pBot && pBot < mTop ||
           pBot <= mBot && mTop <= pTop)
        {
            if (mRig >= pLef && pLef > mLef ||
               mLef <= pRig && pRig < mRig) //적 좌/우에 닿음
            {
                Hit();
                return;
            }
        }

    }
    void Hit()
    {
        player.canmove = false;
        player.vertVec *= -0.5f;
        player.horiVec *= -0.5f;
        
        StartCoroutine(Canmove_after_hit_delay());
    }

    IEnumerator Canmove_after_hit_delay()
    {
        yield return new WaitForSeconds(0.2f);
        player.canmove = true;
    }
}
