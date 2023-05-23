using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager timelineManager { get; private set; } //singleton

    private bool timeIsScaled;

    private void Awake()
    {
        if (timelineManager != null && timelineManager != this)
        {
            Destroy(this);
        }
        else
        {
            timelineManager = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !timeIsScaled)
        {
            Time.timeScale = 5;
            timeIsScaled = true;
        }
        else if (Input.GetKeyDown(KeyCode.T) && timeIsScaled)
        {
            Time.timeScale = 1;
            timeIsScaled = false;
        }
    }
}