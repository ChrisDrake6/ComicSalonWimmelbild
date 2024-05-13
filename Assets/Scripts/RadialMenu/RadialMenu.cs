using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] RadialMenuSO menu;
    [SerializeField] RadialMenuCakePiece cakePiecePrefab;
    [SerializeField] float gapWidthDegree = 1;

    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer body;

    RadialMenuCakePiece[] cakePieces;
    float stepLength;

    bool mouseReleased = false;

    void Start()
    {
        stepLength = 360f / menu.Elements.Length;
        float iconDistance = Vector3.Distance(cakePiecePrefab.Icon.transform.position, cakePiecePrefab.CakePiece.transform.position);

        cakePieces = new RadialMenuCakePiece[menu.Elements.Length];

        for (int i = 0; i < menu.Elements.Length; i++)
        {
            cakePieces[i] = Instantiate(cakePiecePrefab, transform);
            cakePieces[i].transform.localPosition = Vector3.zero;
            cakePieces[i].transform.localRotation = Quaternion.identity;

            cakePieces[i].CakePiece.fillAmount = 1f / menu.Elements.Length - gapWidthDegree / 360f;
            cakePieces[i].CakePiece.transform.localPosition = Vector3.zero;
            cakePieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, stepLength / 2f + gapWidthDegree / 2f + i * stepLength);
            cakePieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            cakePieces[i].Icon.sprite = menu.Elements[i].Icon;
            cakePieces[i].Icon.transform.localPosition = cakePieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDistance;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            mouseReleased = true;
        }
        float mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + stepLength / 2f);
        int activeElementIndex = (int)(mouseAngle / stepLength);

        for (int i = 0; i < menu.Elements.Length; i++)
        {
            if (i == activeElementIndex)
            {
                cakePieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.75f);
            }
            else
            {
                cakePieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }

        if (Input.GetMouseButtonDown(0) && mouseReleased)
        {
            mouseReleased = false;
            switch (menu.Elements[activeElementIndex].Action)
            {
                case PossibleActions.Cancel:
                    RadialMenuManager.Instance.CloseMenu();
                    break;
                case PossibleActions.Leave:
                    RadialMenuManager.Instance.ConfirmSendingSpriteAway();
                    gameObject.SetActive(false);
                    break;
                case PossibleActions.Export:
                    RadialMenuManager.Instance.ExportToGif();
                    break;
            }
        }
    }

    public void Build(SpriteStateManager sprite)
    {
        head.sprite = sprite.data.HeadSprite;
        body.sprite = sprite.data.BodySprite;
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360f) % 360f;
    }
}
