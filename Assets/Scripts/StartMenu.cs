using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Dialog and EnemyHPBar");
    }
    public void LoadLevel()
    {
        //TODO..
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}