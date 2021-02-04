using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int id;
    
    void Start()
    {
        if (id <= GlobalData.GetCheckpointID())
        {
            gameObject.SetActive(false);
        }
    }
}
