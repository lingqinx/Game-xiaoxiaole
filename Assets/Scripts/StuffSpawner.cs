using UnityEngine;
using System.Collections;

public class StuffSpawner : MonoBehaviour
{
    //points where stuff will spawn :)
    public Transform[] StuffSpawnPoints;
    //meat gameobjects
    public GameObject[] Bonus;
    //障碍物
    public GameObject[] Obstacles;

    public bool RandomX = false;
    public float minX = -2f, maxX = 2f;

    // Use this for initialization
    void Start()
    {
        bool placeObstacle = Random.Range(0, 2) == 0; //50% 机率产生障碍物
        int obstacleIndex = -1;
        if (placeObstacle)
        {
			//选择一个随机的产生,除了第一个
            obstacleIndex = Random.Range(1, StuffSpawnPoints.Length);

            CreateObject(StuffSpawnPoints[obstacleIndex].position, Obstacles[Random.Range(0, Obstacles.Length)]);
        }


        for (int i = 0; i < StuffSpawnPoints.Length; i++)
        {
            //如果有障碍物就不实例化
            if (i == obstacleIndex) continue;
            if (Random.Range(0, 3) == 0) //33% 产生糖果们
            {
                CreateObject(StuffSpawnPoints[i].position, Bonus[Random.Range(0, Bonus.Length)]);
            }
        }


    }

    void CreateObject(Vector3 position, GameObject prefab)
    {
        if (RandomX) //产生直线道路
            position += new Vector3(Random.Range(minX, maxX), 0, 0);
		//实例化道路
        Instantiate(prefab, position, Quaternion.identity);
    }


}
