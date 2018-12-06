using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btnStart : MonoBehaviour
{

    void OnMouseDown()
    {
        Destroy(transform.root.gameObject);
    }
}
