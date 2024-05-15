using UnityEngine;


public class RadialMenuManager : MonoBehaviour
{
    [SerializeField] RadialMenu radialMenu;
    [SerializeField] GameObject confirmationDialogue;
    [SerializeField] LineRenderer lineRenderer;

    public static RadialMenuManager Instance { get; private set; }

    SpriteStateManager currentTarget;
    bool isLookingForPartner;

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
        radialMenu.MouseReleased = false;
        radialMenu.gameObject.SetActive(false);
        confirmationDialogue.SetActive(false);
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
        isLookingForPartner = true;
    }

    public void UnLink()
    {
        GroupManager.Instance.RemoveFromGroup(currentTarget);
        CloseMenu();
    }

    private void Update()
    {
        if (radialMenu.gameObject.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            CloseMenu();
        }
        if (isLookingForPartner && Input.GetMouseButtonDown(0))
        {
            GroupManager.Instance.FormGroup(currentTarget);
            isLookingForPartner = false;
            CloseMenu();
        }
    }
}
