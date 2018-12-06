using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Obstacle : MonoBehaviour {

    //box and barrel found here: https://www.assetstore.unity3d.com/en/#!/content/11256

    void OnTriggerEnter(Collider col)
    {
        //如果小车撞到障碍物，游戏结束
        if(col.gameObject.tag == Constants.PlayerTag)
        {
            GameManager.Instance.Die();
        }
    }
}
