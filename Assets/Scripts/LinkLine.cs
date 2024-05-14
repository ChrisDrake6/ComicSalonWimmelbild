using UnityEngine;

public class LinkLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    public SpriteStateManager SelectedSprite;

    public static LinkLine Instance { get; private set; }

    public LinkLine()
    {
        Instance = this;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                lineRenderer.enabled = false;
            }
        }
    }

    public bool IsActive()
    {
        return lineRenderer.enabled;
    }
}
