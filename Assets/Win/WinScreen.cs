using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("x"))
        {
            GlobalData.DeleteCurrentFile();
            SceneManager.LoadScene(GlobalData.GetTitleSceneName());
        }
    }
}
