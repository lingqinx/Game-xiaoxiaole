using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public static Map instance;//单例
    public GameObject prefab;//糖果的预设
    public Sprite[] sp;//糖果的图片 
    public Sweet[,] sweets;//场景中的所有糖果    

    public Text txt_score;//文字 分数目标
    public Text txt_step;//文字 步数

    public GameObject wnd_win;//结束窗口
    public GameObject wnd_start;//开始窗口
    public GameObject wnd_failed;//失败窗口
    public GameObject wnd_timer;//计时器
    public AudioSource audio_Pop;//消除音效

    public bool timeLimit = false;//限制时间
    public bool stepLimit = false;//限制步数
    public int level;//当前的关卡数
    public int targetScore;//目标分数
    public int step;//剩余的步数

    public Text txt_timer;//计时器文字
    public Image img_timer;//计时器条
    public float timeTotal;//限制的时间/秒

    float timer;//游戏时间
    int score;//当前分数
    int stepCount = 0;//记步 -  只有第四关时使用

    void Start()
    {
        //初始化
        instance = this;

        //产生地图
        Creat();

        //如果有三连则直接消除不计分
        if (pair())
            StartCoroutine(Flash(false));

        Time.timeScale = 0;

        if (timeLimit)
        {
            timeTotal = Mathf.Max(1, timeTotal);
            timer = timeTotal;
        }

        //设置UI
        wnd_start.SetActive(true);
        wnd_win.SetActive(false);
        wnd_failed.SetActive(false);
        wnd_timer.SetActive(timeLimit);//如果限制时间 则显示倒计时
    }

    //Okay 按钮 开始游戏
    public void btn_Start()
    {
        wnd_start.SetActive(false);
        canDrag = true;
        Time.timeScale = 1;
    }

    //返回按钮 返回主页面
    public void btn_back()
    {
        SceneManager.LoadScene("Menu");

    }

    //下一关按钮
    public void btn_next()
    {
        if (level != 5)
            SceneManager.LoadScene(level + 1);
    }

    //重试按钮
    public void btn_restart()
    {
        SceneManager.LoadScene(level);
    }

    Sweet lastdrag;//正在拖拽糖果
    Vector3 mouseloc;//鼠标位置
    bool canDrag = true;//是否允许拖拽
    List<Sweet> deleteGroup = new List<Sweet>();//待删除的糖果集合

    void Update()
    {
        UIUpdate();
        //作弊键 按下F1直接胜利
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //胜利 游戏结束
            canDrag = false;
            wnd_win.SetActive(true);
            Time.timeScale = 0;
            //开启新关卡
            PlayerPrefs.SetInt("LEVEL", Mathf.Max(level, PlayerPrefs.GetInt("LEVEL")));
        }

        //如果可以拖拽
        if (canDrag)
        {
            //失败判定
            if ((stepLimit && step <= 0 && score < targetScore) || (timeLimit && timer <= 0))
            {
                //没有完成游戏
                wnd_failed.SetActive(true);
                Time.timeScale = 0;
                return;
            }

            //当按下 鼠标时获取被点击的糖果
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    lastdrag = hit.collider.GetComponent<Sweet>();
                    mouseloc = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
                }
            }
            //当鼠标拖拽时 
            else if (Input.GetMouseButton(0))
            {
                if (lastdrag != null)
                {
                    //判断拖拽的方向
                    Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10) - mouseloc;

                    if (v.x > 0.5f)//向右
                    {
                        if (lastdrag.x < 6)
                        {
                            StartCoroutine(Swap(lastdrag, sweets[lastdrag.x + 1, lastdrag.y], true));
                            lastdrag = null;
                            step--;
                            stepCount++;
                        }
                    }
                    else if (v.x < -0.5f)//向左
                    {
                        if (lastdrag.x > 0)
                        {
                            StartCoroutine(Swap(lastdrag, sweets[lastdrag.x - 1, lastdrag.y], true));
                            lastdrag = null;
                            step--;
                            stepCount++;
                        }
                    }
                    else if (v.y > 0.5f)//向上
                    {
                        if (lastdrag.y < 6)
                        {
                            StartCoroutine(Swap(lastdrag, sweets[lastdrag.x, lastdrag.y + 1], true));
                            lastdrag = null;
                            step--;
                            stepCount++;
                        }
                    }
                    else if (v.y < -0.5f)//向下
                    {
                        if (lastdrag.y > 0)
                        {
                            StartCoroutine(Swap(lastdrag, sweets[lastdrag.x, lastdrag.y - 1], true));
                            lastdrag = null;
                            step--;
                            stepCount++;
                        }
                    }
                }
            }
            //松开鼠标时
            else if (Input.GetMouseButtonUp(0))
            {
                lastdrag = null;
            }
        }

    }

    void UIUpdate()
    {
        //更新显示步数
        if (stepLimit)
            txt_step.text = step.ToString();
        else
            txt_step.text = "∞";

        //倒计时
        if (timeLimit)
        {
            timer = Mathf.Max(0, timer - Time.deltaTime);
            int minute = Mathf.RoundToInt(timer) / 60;
            int second = Mathf.RoundToInt(timer) % 60;
            txt_timer.text = "" + minute + ":" + second.ToString("D2");//显示为X:XX
            img_timer.fillAmount = timer / timeTotal;
        }

        //显示分数
        txt_score.text = "" + score;
        if (score >= targetScore)
        {
            //胜利 游戏结束
            canDrag = false;
            wnd_win.SetActive(true);
            Time.timeScale = 0;
            //开启新关卡
            PlayerPrefs.SetInt("LEVEL", Mathf.Max(level, PlayerPrefs.GetInt("LEVEL")));
        }
    }

    //计分规则
    //不同关卡
    void ScoreRule(Sweet sw)
    {
        switch (level)
        {
            case 1://关卡1 所有糖果算1分
                score++;//
                break;
            case 2://关卡2 红=1 蓝=2 橙=3 紫=0
                if (sw.type != 3)
                    score += (1 + sw.type);
                break;
            case 3://关卡3 红=1 蓝=0 橙=0 紫=0 //限制步数
                if (sw.type == 0)
                    score += 1;
                break;
            case 4://关卡4 红=1 蓝=2 橙=3 紫=0 //随机刷新地图 //限制时间
                if (sw.type != 3)
                    score += (1 + sw.type);
                break;
            case 5://关卡5 红=0 蓝=1 橙=0 紫=0 //限制步数 限制时间
                if (sw.type == 1)
                    score += 1;
                break;
        }
    }


    //随机产生7*7的糖果地图
    void Creat()
    {
        sweets = new Sweet[7, 7];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                int type = Random.Range(0, sp.Length);
                GameObject gobj = Instantiate(prefab, transform.position + new Vector3(i - 3, j + 4, 0), Quaternion.identity, transform);
                Sweet sweet = gobj.GetComponent<Sweet>();
                sweet.type = type;
                sweet.x = i;
                sweet.y = j;
                sweets[i, j] = sweet;
            }
        }
    }

    //更新地图
    IEnumerator Flash(bool countScore)
    {
        //拒绝拖拽
        canDrag = false;
        if (deleteGroup.Count > 0)
        {
            //统计每一列删除的个数
            int[] linedelete = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            //删除掉三连及以上的糖果
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (deleteGroup.Contains(sweets[i, j]))
                    {
                        linedelete[i]++;
                        if (countScore)
                        {
                            //计分
                            ScoreRule(sweets[i, j]);
                            sweets[i, j].delete();
                        }
                        else
                        {
                            Destroy(sweets[i, j].gameObject);
                        }
                        sweets[i, j] = null;
                    }
                }
            }
            //每一列的空位下落
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Sweet sw = sweets[i, j];
                    if (sw)
                    {
                        while (sw.y > 0 && !sweets[i, sw.y - 1])
                        {
                            sweets[i, sw.y - 1] = sw;
                            sweets[i, sw.y] = null;
                            sw.y--;
                        }
                    }
                }
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < linedelete[i]; j++)
                {
                    //在每一列的顶端产生新糖果
                    int type = Random.Range(0, sp.Length);
                    GameObject gobj = Instantiate(prefab, transform.position + new Vector3(i - 3, 4 + j, 0), Quaternion.identity, transform);
                    Sweet sweet = gobj.GetComponent<Sweet>();
                    sweet.type = type;
                    sweet.x = i;
                    sweet.y = 7 - linedelete[i] + j;
                    sweets[i, 7 - linedelete[i] + j] = sweet;
                }
            }

            //如果计分则播放消除/产生/下落的动画 
            if (countScore)
            {
                yield return new WaitForSeconds(0.5f);
                //重新移动场景
                foreach (var sw in sweets)
                {
                    // sw.transform.localPosition = new Vector3(sw.x - 3, sw.y - 3, 0);
                    sw.Move(new Vector3(sw.x - 3, sw.y - 3, 0));
                }
                yield return new WaitForSeconds(0.5f);
            }
            else//如果不计分则立即更新场景
            {
                //重新移动场景
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        sweets[i, j].transform.localPosition = new Vector3(i - 3, j - 3, 0);
                    }
                }
            }
            //判断新产生的场景是否有3连 (迭代
            if (pair())
            {
                StartCoroutine(Flash(countScore));
                //如果计分则播放音效
                if (countScore)
                    audio_Pop.Play();
            }
            else
            {
                //新场景没有新三联 开始允许拖拽 
                AutoChange();
            }
        }
        else
        {
            //没有需要删除的糖果 直接允许拖拽 拖拽
            AutoChange();
        }
    }

    //拖拽糖果、交换后判断是否存在消除可能
    IEnumerator Swap(Sweet a, Sweet b, bool first)
    {
        canDrag = false;
        //交换
        yield return new WaitForEndOfFrame();
        int x = a.x;
        int y = a.y;
        sweets[a.x, a.y] = b;
        sweets[b.x, b.y] = a;
        a.x = b.x;
        a.y = b.y;
        b.x = x;
        b.y = y;
        //重新移动场景
        foreach (var sw in sweets)
        {
            // sw.transform.localPosition = new Vector3(sw.x - 3, sw.y - 3, 0);
            sw.Move(new Vector3(sw.x - 3, sw.y - 3, 0));
        }
        yield return new WaitForSeconds(0.2f);
        //交换第一次没有匹配之后 再次交换 只有第一次判断是否匹配
        if (first)
        {
            if (pair())
            {
                //有匹配
                StartCoroutine(Flash(true));
                //播放音效
                audio_Pop.Play();
            }
            else
            {
                //无匹配 再次交换
                StartCoroutine(Swap(a, b, false));
                yield return new WaitForSeconds(0.2f);
                AutoChange();
            }
        }
        else
        {
            //交换后无匹配 再交换回来重新允许拖拽
            yield return new WaitForSeconds(0.2f);
            AutoChange();
        }
    }

    //让地图中的一部分糖果 随机变色
    void AutoChange()
    {
        Debug.Log(stepCount);
        if (level != 4)//第四关以外的关卡不会触发
        {
            canDrag = true;
            return;
        }
        else
        {
            if (stepCount == 5)//玩家每走5步 触发一次自动刷新
            {
                StartCoroutine(doChange());
                stepCount = 0;
            }
            else
            {
                canDrag = true;
            }
        }

    }


    IEnumerator doChange()
    {
        int i;
        for (int k = 0; k < Random.Range(2, 3); k++)//随机将 两行/列 变为没有分的糖果
        {
            i = Random.Range(0, 7);
            if (Random.value > 0.5f)
            {
                for (int j = 0; j < 7; j++)
                {
                    yield return new WaitForSeconds(0.2f);
                    sweets[i, j].type = Random.Range(0,4);//变为随机糖果
                    sweets[i, j].SetImage();
                    sweets[i, j].change();
                    audio_Pop.Play();
                }
            }
            else
            {
                for (int j = 0; j < 7; j++)
                {
                    yield return new WaitForSeconds(0.2f);
                    sweets[j, i].type = Random.Range(0, 4);//变为随机糖果
                    sweets[j, i].SetImage();
                    sweets[j, i].change();
                    audio_Pop.Play();
                }
            }

        }
        yield return new WaitForSeconds(0.2f);

        if (pair())
        {
            //有匹配
            StartCoroutine(Flash(true));
            //播放音效
            audio_Pop.Play();
        }
    }

    //判断地图上是否存在3连
    public bool pair()
    {
        deleteGroup = new List<Sweet>();
        //判断需要消除的格子
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                //判断任一糖果是否同时和左右相同
                if (i > 0 && i < 6
                    && sweets[i - 1, j].type == sweets[i, j].type
                    && sweets[i + 1, j].type == sweets[i, j].type)
                {
                    deleteGroup.Add(sweets[i, j]);
                    deleteGroup.Add(sweets[i - 1, j]);
                    deleteGroup.Add(sweets[i + 1, j]);
                }

                //判断任一糖果是否同时和上下相同
                if (j > 0 && j < 6
                    && sweets[i, j - 1].type == sweets[i, j].type
                    && sweets[i, j + 1].type == sweets[i, j].type)
                {
                    deleteGroup.Add(sweets[i, j]);
                    deleteGroup.Add(sweets[i, j - 1]);
                    deleteGroup.Add(sweets[i, j + 1]);
                }
            }
        }
        return deleteGroup.Count > 0;
    }
}