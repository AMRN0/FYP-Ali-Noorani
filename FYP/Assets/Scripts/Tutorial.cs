using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public bool inGame = false;

    public GameObject step1, step2, step3, step4, step4_2, step5;
    public TMP_Text text;
    public Button nextButton, previousButton;
    private int index = 0;
    private bool tutorial;
    private int maxSteps = 5;

    private void Awake()
    {

        if (inGame)
        {
            return;
        }

        if (PlayerPrefs.HasKey("Tutorial"))
        {
            if (PlayerPrefs.GetInt("Tutorial") >= 1)
            {
                SkipTutorial();
                return;
            }
            tutorial = true;
        }
        else
        {
            PlayerPrefs.SetInt("Tutorial", 0);
            tutorial = true;
        }

        PlayerPrefs.Save();
    }

    private void Start()
    {
        if (inGame)
        {
            return;
        }
        if (tutorial)
        {
            index = 0;
            step1.SetActive(true);
            step2.SetActive(false);
            step3.SetActive(false);
            step4.SetActive(false);
            step4_2.SetActive(false);
            step5.SetActive(false);
        }
    }

    private void Update()
    {

        if (inGame)
        {
            return;
        }
        if (tutorial)
        {
            switch (index)
            {
                case 0:
                    step1.SetActive(true);
                    step2.SetActive(false);
                    text.text = "The top dropdown is to select your current location. The start position.";
                    break;
                case 1:
                    step1.SetActive(false);
                    step3.SetActive(false);
                    step2.SetActive(true);
                    text.text = "The dropdown below it is to select your destination. Where you want to go.";
                    break;
                case 2:
                    step4.SetActive(false);
                    step4_2.SetActive(false);
                    step2.SetActive(false);
                    step3.SetActive(true);
                    text.text = "This is the pause and play button. You can press this once you have selected your start and end locations. Once pressed, the line will start or stop moving";
                    break;
                case 3:
                    step3.SetActive(false);
                    step5.SetActive(false);
                    step4.SetActive(true);
                    step4_2.SetActive(true);
                    text.text = "These two buttons are to look ahead or back of the route. They help you see where you need to go before you start moving. You can press the play button to re-center yourself.";
                    break;
                case 4:
                    step5.SetActive(true);
                    step4.SetActive(false);
                    step4_2.SetActive(false);
                    text.text = "This is the settings menu where you can change how quickly the line moves, and toggle between stairs or lifts";
                    break;
                case 5:
                    step5.SetActive(false);
                    text.text = "You can also look around by swiping left or right. Now, try using the main app. Press Next to go to the main app.";
                    break;
                default:
                    break;
            }
        }
    }

    public void LoadScene()
    {
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    public void StepOver(int step)
    {
        index += step;

        if (index < 0)
        {
            index = 0;
        }
        else if (index > maxSteps)
        {
            SkipTutorial();
        }
    }

    public void SkipTutorial()
    {
        nextButton.interactable = false;
        previousButton.interactable = false;
        tutorial = false;
        PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.Save();
    }
}
