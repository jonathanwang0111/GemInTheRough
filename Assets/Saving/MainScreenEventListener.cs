using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MainScreenEventListener : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject menuPanel;

    // Used for keeping track of Current Screen
    private enum screen
    {
        Title,
        Menu
    }
    private screen currentScreen;

    // Main Menu Variables
    private enum menuChoices
    {
        saveFileOne,
        saveFileTwo,
        saveFileThree,
        numChoices
    }
    private menuChoices currentMenuChoice;
    public TextMeshProUGUI[] menuChoiceText;

    // Start is called before the first frame update
    void Start()
    {
        currentScreen = screen.Title;
        titlePanel.SetActive(true);
        menuPanel.SetActive(false);

        currentMenuChoice = menuChoices.saveFileOne;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScreen == screen.Title)
        {
            if (Input.GetKeyDown("x"))
            {
                TransitionScreen(screen.Menu);
            }
        }
        else if (currentScreen == screen.Menu)
        {
            if (Input.GetKeyDown("b"))
            {
                TransitionScreen(screen.Title);
            }

            if (Input.GetKeyDown("x"))
            {
                PressPlay();
            }

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
                if (i == (int)currentMenuChoice)
                {
                    menuChoiceText[i].color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    menuChoiceText[i].color = new Color32(255, 255, 255, 100);
                }
            }
        }
    }

    void TransitionScreen(screen newScreen)
    {
        if (newScreen == screen.Menu)
        {
            titlePanel.SetActive(false);
            menuPanel.SetActive(true);
            currentMenuChoice = menuChoices.saveFileOne;
        }

        if (newScreen == screen.Title)
        {
            titlePanel.SetActive(true);
            menuPanel.SetActive(false);
        }

        currentScreen = newScreen;
    }

    void PressPlay()
    {
        GlobalData.LoadData((int) currentMenuChoice);
        SceneManager.LoadScene(GlobalData.GetLevelName());
    }
}
