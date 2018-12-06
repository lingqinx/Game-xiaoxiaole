using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BGM保持唯一
public class BGM : MonoBehaviour
{
    public static BGM instance;//单例模式

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//切换场景也不删除
        }
    }
}
