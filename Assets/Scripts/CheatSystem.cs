using UnityEngine;

public class CheatSystem : MonoBehaviour
{
    public GameObject Background;
    public GameObject Player;
    private bool invincible;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !invincible)
        {
            //Background.SetActive(!Background.activeSelf);
            Player.GetComponent<PlayerHealth>().AddInvulnerability();
            invincible = true;
        }
        else if (Input.GetKeyDown(KeyCode.F1) && invincible)
        {
            //Background.SetActive(!Background.activeSelf);
            Player.GetComponent<PlayerHealth>().RemoveInvulnerability();
            invincible = false;
        }
    }
}