using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataType
{
    public float[] checkpoint;
    public int checkPointId;

    public SaveDataType(float[] pt, int id)
    {
        checkpoint = pt;
        checkPointId = id;
    }
}
