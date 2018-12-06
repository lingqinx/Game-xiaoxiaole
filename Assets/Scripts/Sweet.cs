using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweet : MonoBehaviour
{
    public int type;
    public int x;
    public int y;

    //设置图片
    void Start()
    {
        SetImage();
    }

    public void SetImage()
    {
        GetComponent<SpriteRenderer>().sprite = Map.instance.sp[type];
    }

    //移动到指定位置
    public void Move(Vector3 position)
    {
        StartCoroutine(doMove(position));
    }

    //逐渐移动
    IEnumerator doMove(Vector3 position)
    {
        Vector3 a = transform.localPosition;
        Vector3 b = position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(a, b, t);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = position;
    }

    //消除糖果
    public void delete()
    {
        StartCoroutine(doDel());
    }

    //用脚本实现的消除动画 先变大再变小消失
    IEnumerator doDel()
    {
        float f = 1f;
        //先变大
        while (f < 1.2f)
        {
            f += Time.deltaTime*5;
            transform.localScale = new Vector3(f, f, f);
            yield return new WaitForEndOfFrame();
        }
        //再变小
        while (f > 0)
        {
            f -= Time.deltaTime*5;
            transform.localScale = new Vector3(f, f, f);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void change()
    {
        StartCoroutine(doChange());
    }

    IEnumerator doChange()
    {
        float f = 0;
        //先变大
        while (f <1f)
        {
            f += Time.deltaTime * 5;
            transform.localScale = new Vector3(f, f, f);
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = Vector3.one;
    }
}
