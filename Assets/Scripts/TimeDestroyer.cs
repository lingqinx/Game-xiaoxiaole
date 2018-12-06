using UnityEngine;
using System.Collections;

public class TimeDestroyer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Invoke("DestroyObject", LifeTime);
    }

	//如果死亡，销毁gameObject
    void DestroyObject()
    {
        if (GameManager.Instance.GameState != GameState.Dead)
            Destroy(gameObject);
    }


    public float LifeTime = 10f;
}
