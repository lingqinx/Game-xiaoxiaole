using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//屏幕上显示当状态／信息，实时分数
public class UIManager : MonoBehaviour
{
	int level;//当前可以玩的level
    void Awake()
	{	level = PlayerPrefs.GetInt("LEVEL", 0);//读取存档 开启按钮
		//Debug.Log (level);
        if (instance == null)
        {
            instance = this;
        }
        else
        {//消除当instance存在
            DestroyImmediate(this);
        }
    }

    //singleton implementation
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            
            return instance;
        }
    }

    private float scored = 0;//分数

	//重置分数
    public void ResetScore()
    {
        scored = 0;
        UpdateScoreText();
    }
    //实时得到的分数
    public void SetScore(float value)
    {
        scored = value;
        UpdateScoreText();
    }
	//累计分数加上当前得到分数
    public void IncreaseScore(float value)
    {
        scored += value;
        UpdateScoreText();
    }
	//显示分数，如果分数到达10000,可以开启一下关
    private void UpdateScoreText()
	{	if (scored > 10000 && level<5) {
			//for (int level = 0; level < 6; level++) {
			//Debug.Log (level);
			PlayerPrefs.SetInt("LEVEL", Mathf.Max(level+1, PlayerPrefs.GetInt("LEVEL")));
			SceneManager.LoadScene(level+2);
			//}
		}
		
        ScoreText.text = "Score: "+scored.ToString();
    }
	//显示当前游戏状态
    public void SetStatus(string text)
    {
        StatusText.text = text;
    }

    public Text ScoreText, StatusText;
	//返回按钮 返回主页面
	public void btn_back()
	{
		SceneManager.LoadScene("Menu");
	}


}
