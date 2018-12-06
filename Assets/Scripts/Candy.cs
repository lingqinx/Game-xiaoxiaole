using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Candy : MonoBehaviour
{
    //candy found in https://www.assetstore.unity3d.com/en/#!/content/12512

    // Update is called once per frame
    void Update()
	{//实现糖块的旋转效果
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }
	// 碰到糖果加上相应分数
    void OnTriggerEnter(Collider col)
    {
        UIManager.Instance.IncreaseScore(ScorePoints);
        Destroy(this.gameObject);
    }

	public int ScorePoints = 100;//每个糖果分数
	public float rotateSpeed = 50f;//糖果自身旋转的速度
}
