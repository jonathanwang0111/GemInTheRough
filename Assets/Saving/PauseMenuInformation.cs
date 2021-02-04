using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuInformation : MonoBehaviour
{
    public GameObject pauseMenu;
    public Transform startingPoint;
    private float[] checkpoint;
    private int checkpointID;
    private bool gamePaused;

    private enum menuChoices
    {
        resume,
        saveAndQuit,
        numChoices
    }
    private menuChoices currentMenuChoice;
    public TextMeshProUGUI[] menuChoiceText;

    void Start()
    {
        Resume();
        if (GlobalData.GetCheckpoint()[0] != 0 ||
            GlobalData.GetCheckpoint()[1] != 0 ||
            GlobalData.GetCheckpoint()[2] != 0)
        {
            checkpoint = GlobalData.GetCheckpoint();
            checkpointID = GlobalData.GetCheckpointID();
        }
        else
        {
            checkpoint = new float[3];
            checkpoint[0] = startingPoint.position.x;
            checkpoint[1] = startingPoint.position.y;
            checkpoint[2] = startingPoint.position.z;
            checkpointID = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (gamePaused)
        {
            if (Input.GetKeyDown("up"))
            {
                if ((int)currentMenuChoice != 0)
                {
                    currentMenuChoice = (menuChoices)(currentMenuChoice - 1);
                }
            }

            if (Input.GetKeyDown("down"))
            {
                if ((int)currentMenuChoice != (int)menuChoices.numChoices - 1)
                {
                    currentMenuChoice = (menuChoices)(currentMenuChoice + 1);
                }
            }

            for (int i = 0; i < menuChoiceText.Length; i++)
            {
                if (i == (int) currentMenuChoice)
                {
                    menuChoiceText[i].color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    menuChoiceText[i].color = new Color32(255, 255, 255, 100);
                }
            }

            if (Input.GetKeyDown("x"))
            {
                if (currentMenuChoice == menuChoices.resume)
                {
                    Resume();
                }
                else if (currentMenuChoice == menuChoices.saveAndQuit)
                {
                    SaveAndQuit();
                }
            }
        }
    }

    void Resume()
    {
        gamePaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void Pause()
    {
        gamePaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        currentMenuChoice = menuChoices.resume;
    }

    void SaveAndQuit()
    {
        GlobalData.SaveData(checkpoint, checkpointID);
        SceneManager.LoadScene(GlobalData.GetTitleSceneName());
    }

    public void SetCheckpoint(float[] pt, int id)
    {
        checkpoint = pt;
        checkpointID = id;
    }
}
