using UnityEngine;


public class RadialMenuManager : MonoBehaviour
{
    [SerializeField] RadialMenu radialMenu;
    [SerializeField] GameObject confirmationDialogue;
    [SerializeField] LineRenderer lineRenderer;

    public static RadialMenuManager Instance { get; private set; }

    SpriteStateManager currentTarget;

    public RadialMenuManager()
    {
        Instance = this;
    }

    public void OnSpriteClick(SpriteStateManager sprite)
    {
        if (!lineRenderer.enabled)
        {
            Time.timeScale = 0;
            currentTarget = sprite;
            radialMenu.gameObject.SetActive(true);
            radialMenu.Build(sprite);
        }
        else
        {
            GroupManager.Instance.FormGroup(currentTarget);
            CloseMenu();
        }
    }

    public void CloseMenu(bool resume = true)
    {
        if (resume)
        {
            Time.timeScale = 1;
        }
        radialMenu.gameObject.SetActive(false);
        confirmationDialogue.SetActive(false);
        lineRenderer.enabled = false;
    }

    public void ConfirmSendingSpriteAway()
    {
        confirmationDialogue.SetActive(true);
        radialMenu.gameObject.SetActive(false);
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

    public void InitiateLink()
    {
        radialMenu.gameObject.SetActive(false);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, currentTarget.transform.position);
    }

    public void UnLink()
    {
        GroupManager.Instance.RemoveFromGroup(currentTarget);
        CloseMenu();
    }

    private void Update()
    {
        if ((radialMenu.gameObject.activeInHierarchy || confirmationDialogue.activeInHierarchy || lineRenderer.enabled) && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            CloseMenu(!Input.GetKeyDown(KeyCode.Escape));
        }

    }
}
