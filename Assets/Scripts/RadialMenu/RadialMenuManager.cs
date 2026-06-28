using UnityEngine;


public class RadialMenuManager : MonoBehaviour
{
    [SerializeField] RadialMenu radialMenu;
    [SerializeField] GameObject confirmationDialogue;

    public static RadialMenuManager Instance { get; private set; }

    PlayerFigureController currentTarget;

    public RadialMenuManager()
    {
        Instance = this;
    }

    public void OnFigureClick(PlayerFigureController figure)
    {
        Time.timeScale = 0;
        GameManager.Instance.Paused = true;
        currentTarget = figure;
        radialMenu.gameObject.SetActive(true);
        radialMenu.Build(figure);
    }

    public void CloseMenu(bool resume = true)
    {
        radialMenu.gameObject.SetActive(false);
        confirmationDialogue.SetActive(false);
        if (resume)
        {
            Time.timeScale = 1;
            GameManager.Instance.Paused = false;
        }
    }

    public void ConfirmSendingSpriteAway()
    {
        confirmationDialogue.SetActive(true);
        radialMenu.gameObject.SetActive(false);
    }

    public void SendSpriteAway()
    {
        currentTarget.StartLeaving();
        CloseMenu();
    }

    private void Update()
    {
        if ((radialMenu.gameObject.activeInHierarchy || confirmationDialogue.activeInHierarchy) && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            CloseMenu(!Input.GetKeyDown(KeyCode.Escape));
        }
    }
}
