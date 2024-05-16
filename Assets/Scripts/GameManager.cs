using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseWindow;
    [SerializeField] GameObject confirmationWindow;

    public bool Paused { get; set; }

    public static GameManager Instance { get; private set; }

    public GameManager()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseWindow.activeInHierarchy || confirmationWindow.activeInHierarchy)
            {
                ResumeApplication();
            }
            else
            {
                Paused = true;
                pauseWindow.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void ResumeApplication()
    {
        Paused = false;
        Time.timeScale = 1;
        pauseWindow.SetActive(false);
        confirmationWindow.SetActive(false);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
