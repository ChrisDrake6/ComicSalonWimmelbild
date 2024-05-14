using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] RadialMenuSO menu;
    [SerializeField] RadialMenuCakePiece cakePiecePrefab;
    [SerializeField] float gapWidthDegree = 1;

    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer body;

    List<RadialMenuCakePiece> cakePieces = new List<RadialMenuCakePiece>();
    float stepLength;

    public bool MouseReleased = false;
    public bool isBuilt = false;

    int piecesCount;

    public void Build(SpriteStateManager sprite)
    {
        isBuilt = false;
        foreach (RadialMenuCakePiece piece in cakePieces)
        {
            Destroy (piece.gameObject);
        }

        piecesCount = menu.Elements.Length - 1;
        stepLength = 360f / (piecesCount);
        cakePieces = new List<RadialMenuCakePiece>();
        float iconDistance = Vector3.Distance(cakePiecePrefab.Icon.transform.position, cakePiecePrefab.CakePiece.transform.position);

        for (int i = 0; i < menu.Elements.Length; i++)
        {
            if (!(menu.Elements[i].Action == PossibleActions.Link && sprite.isInGroup) && !(menu.Elements[i].Action == PossibleActions.Unlink && !sprite.isInGroup))
            {
                RadialMenuCakePiece element = Instantiate(cakePiecePrefab, transform);
                element.transform.localPosition = Vector3.zero;
                element.transform.localRotation = Quaternion.identity;

                element.CakePiece.fillAmount = 1f / piecesCount - gapWidthDegree / 360f;
                element.CakePiece.transform.localPosition = Vector3.zero;
                element.CakePiece.transform.localRotation = Quaternion.Euler(0, 0, stepLength / 2f + gapWidthDegree / 2f + cakePieces.Count * stepLength);
                element.CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

                element.Icon.sprite = menu.Elements[i].Icon;
                element.Icon.transform.localPosition = element.CakePiece.transform.localPosition + Quaternion.AngleAxis(cakePieces.Count * stepLength, Vector3.forward) * Vector3.up * iconDistance;

                element.Action = menu.Elements[i].Action;
                cakePieces.Add(element);
            }
        }

        head.sprite = sprite.data.HeadSprite;
        body.sprite = sprite.data.BodySprite;

        isBuilt = true;
    }

    private void Update()
    {
        if (isBuilt)
        {            
            if (Input.GetMouseButtonUp(0))
            {
                MouseReleased = true;
            }
            float mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + stepLength / 2f);
            int activeElementIndex = (int)(mouseAngle / stepLength);

            for (int i = 0; i < piecesCount; i++)
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

            if (Input.GetMouseButtonDown(0) && MouseReleased)
            {
                switch (cakePieces[activeElementIndex].Action)
                {
                    case PossibleActions.Cancel:
                        RadialMenuManager.Instance.CloseMenu();
                        break;
                    case PossibleActions.Leave:
                        RadialMenuManager.Instance.ConfirmSendingSpriteAway();
                        break;
                    case PossibleActions.Export:
                        RadialMenuManager.Instance.ExportToGif();
                        break;
                    case PossibleActions.Link:
                        RadialMenuManager.Instance.InitiateLink();
                        break;
                    case PossibleActions.Unlink:
                        RadialMenuManager.Instance.UnLink();
                        break;
                }
            }
        }
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360f) % 360f;
    }
}
