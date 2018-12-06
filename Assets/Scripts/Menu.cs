using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator anim;//开始界面的动画 包含start界面 和 
    public Button[] buttons;//选择关卡按钮
    void Start()
    {
        //PC端 设置窗口分辨率
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            Screen.SetResolution(9 * 50, 16 * 50, false);//保证9:16的分辨率
        //读取存档 开启按钮
        int level = PlayerPrefs.GetInt("LEVEL", 0);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = i <= level;
        }
        Time.timeScale = 1;
		PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        //按下F12重置存档
        if (Input.GetKeyDown(KeyCode.F12))
        {
            //删除掉保存的数据,重新开始游戏
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }
    }

    //开始按钮
    public void btn_start()
    {
        //切换到选择关卡界面
        anim.CrossFade("Select",0.5f);
    }

    //返回按钮
    public void btn_back()
    {
        //切换到开始界面
        anim.CrossFade("Start", 0.5f);
    }

    //选择关卡按钮
    public void btn_level(int index)
    {
        SceneManager.LoadScene(index);
    }
	public void btn_3d()
	{
		SceneManager.LoadScene("straightPaths");
	}

    //退出游戏按钮
    public void btn_quit()
    {
        Application.Quit();
    }
}
