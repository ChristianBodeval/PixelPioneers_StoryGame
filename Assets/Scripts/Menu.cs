using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf)
            {
                TurnMenuOff();
            }
            else
            {
                TurnMenuOn();
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void TurnMenuOn()
    {
        menu.SetActive(true);
    }

    private void TurnMenuOff()
    {
        menu.SetActive(true);
    }
}
