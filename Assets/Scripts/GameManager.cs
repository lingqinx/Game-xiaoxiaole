using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
		{//消除当instance存在
            DestroyImmediate(this);
        }
    }
    

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }
	//设置最初不能滑动
    protected GameManager()
    {
        GameState = GameState.Start;
        CanSwipe = false;
    }

    public GameState GameState { get; set; }

    public bool CanSwipe { get; set; }
    //设置死亡状态并输出fail信息
    public void Die()
    {
            UIManager.Instance.SetStatus(Constants.StatusDeadTapToStart);
            this.GameState = GameState.Dead; 
    }

}



