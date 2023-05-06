using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    private MenuState menuState;
    private bool isPaused = false;
    private int input = 0;

    [Header("Menu Images")]
    public Image resume;

    public Image volume;
    public Image exit;

    public Material selected;
    public Material unSelected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            TurnMenuOn();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            TurnMenuOff();
        }

        if (Input.GetKeyDown(KeyCode.W) && isPaused)
        {
            input--;
        }
        else if (Input.GetKeyDown(KeyCode.S) && isPaused)
        {
            input++;
        }

        if (input > 2)
        {
            input = 0;
        }
        else if (input < 0)
        {
            input = 2;
        }

        //Safety, without this 2 buttons can be selected at once
        if (input == 2)
            resume.material = unSelected;

        menuState = (MenuState)input;

        if (isPaused)
        {
            switch (menuState)
            {
                case MenuState.Resume:
                    resume.material = selected;
                    volume.material = unSelected;
                    exit.material = unSelected;

                    break;

                case MenuState.Volume:
                    resume.material = unSelected;
                    volume.material = selected;
                    exit.material = unSelected;
                    break;

                case MenuState.Exit:
                    exit.material = unSelected;
                    volume.material = unSelected;
                    exit.material = selected;
                    break;
            }
        }

        //Press the selected option
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (menuState)
            {
                case MenuState.Resume:
                    TurnMenuOff();
                    break;

                case MenuState.Volume:

                    break;

                case MenuState.Exit:
                    ExitGame();
                    break;
            }
        }
    }

    private enum MenuState
    {
        Resume,
        Volume,
        Exit
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void TurnMenuOn()
    {
        menu.SetActive(true);
        isPaused = true;

        Time.timeScale = 0;
    }

    private void TurnMenuOff()
    {
        menu.SetActive(false);
        isPaused = false;

        Time.timeScale = 1;
    }
}