using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    private MenuState menuState;
    private bool isPaused = false;
    private int input = 0;

    [Header("Menu Images")]
    public SpriteRenderer newGame;
    public SpriteRenderer loadGame;
    public SpriteRenderer exit;

    public Material selected;
    public Material unSelected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            input--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
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
            newGame.material = unSelected;

        menuState = (MenuState)input;

        switch (menuState)
        {
            case MenuState.NewGame:
                newGame.material = selected;
                loadGame.material = unSelected;
                exit.material = unSelected;

                break;

            case MenuState.LoadGame:
                newGame.material = unSelected;
                loadGame.material = selected;
                exit.material = unSelected;
                break;

            case MenuState.Exit:
                exit.material = unSelected;
                loadGame.material = unSelected;
                exit.material = selected;
                break;
        }

        //Press the selected option
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (menuState)
            {
                case MenuState.NewGame:
                    SceneManager.LoadScene("Fathers House");
                    break;

                case MenuState.LoadGame:

                    break;

                case MenuState.Exit:
                    ExitGame();
                    break;
            }
        }
    }

    private enum MenuState
    {
        NewGame,
        LoadGame,
        Exit
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}