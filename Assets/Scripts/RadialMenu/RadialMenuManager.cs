using UnityEngine;


public class RadialMenuManager : MonoBehaviour
{
    [SerializeField] RadialMenu radialMenu;
    [SerializeField] GameObject confirmationDialogue;

    public static RadialMenuManager Instance { get; private set; }

    SpriteStateManager currentTarget;


    public RadialMenuManager()
    {
        Instance = this;
    }

    public void OnSpriteClick(SpriteStateManager sprite)
    {
        Time.timeScale = 0;
        currentTarget = sprite;
        radialMenu.gameObject.SetActive(true);
        radialMenu.Build(sprite);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        radialMenu.gameObject.SetActive(false);
        confirmationDialogue.SetActive(false);
    }

    public void ConfirmSendingSpriteAway()
    {
        confirmationDialogue.SetActive(true);
    }

    public void SendSpriteAway()
    {
        currentTarget.SwitchState(currentTarget.leavingState);
        CloseMenu();
    }

    public void ExportToGif() 
    {
        // TODO: Export to gif
    }

    private void Update()
    {
        if (radialMenu.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
