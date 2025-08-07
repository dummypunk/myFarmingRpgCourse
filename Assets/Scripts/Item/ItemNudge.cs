using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isAnimating == false)
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }

    private IEnumerator RotateAntiClock()
    {
        //当玩家从右边接触物体或者从左边离开物体时，物品会首先进行逆时针旋转，随后进行顺时针旋转，最后回归初始的rotate，产生类似弹簧的效果
        //利用Couroutine来进行逐渐旋转的处理（利用for循环中的yield return关键符来进行延时操作）
        isAnimating = true;

        //通过for循环使得gameObject进行逆时针旋转
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, 2f);
            //进行短暂的停顿来使得for循环看起来有一定的连续性
            yield return pause;
        }

        //顺时针旋转
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, -2f);
            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0, 0, 2f);

        yield return pause;
        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        {
            isAnimating = true;

            //通过for循环使得gameObject进行逆时shun旋转
            for (int i = 0; i < 4; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0, 0, -2f);
                //进行短暂的停顿来使得for循环看起来有一定的连续性
                yield return pause;
            }

            //顺时针旋转
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0, 0, 2f);
                yield return pause;
            }

            gameObject.transform.GetChild(0).Rotate(0, 0, -2f);

            yield return pause;
            isAnimating = false;
        }
    }
}
