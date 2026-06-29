using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private RadialMenuSO menu;
    [SerializeField] private RadialMenuCakePiece cakePiecePrefab;
    [SerializeField] private float gapWidthDegree = 1;

    [SerializeField] private SpriteRenderer head;
    [SerializeField] private SpriteRenderer body;

    private List<RadialMenuCakePiece> _cakePieces = new List<RadialMenuCakePiece>();
    private float _stepLength;

    private bool _isBuilt = false;

    int piecesCount;

    public void Build(PlayerFigureController figure)
    {
        _isBuilt = false;
        foreach (RadialMenuCakePiece piece in _cakePieces)
        {
            Destroy(piece.gameObject);
        }

        piecesCount = menu.Elements.Length;
        _stepLength = 360f / (piecesCount);
        _cakePieces = new List<RadialMenuCakePiece>();
        float iconDistance = Vector3.Distance(cakePiecePrefab.Icon.transform.position, cakePiecePrefab.CakePiece.transform.position);

        for (int i = 0; i < menu.Elements.Length; i++)
        {
            RadialMenuCakePiece element = Instantiate(cakePiecePrefab, transform);
            element.transform.localPosition = Vector3.zero;
            element.transform.localRotation = Quaternion.identity;

            element.CakePiece.fillAmount = 1f / piecesCount - gapWidthDegree / 360f;
            element.CakePiece.transform.localPosition = Vector3.zero;
            element.CakePiece.transform.localRotation = Quaternion.Euler(0, 0, _stepLength / 2f + gapWidthDegree / 2f + _cakePieces.Count * _stepLength);
            element.CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            element.Icon.sprite = menu.Elements[i].Icon;
            element.Icon.transform.localPosition = element.CakePiece.transform.localPosition + Quaternion.AngleAxis(_cakePieces.Count * _stepLength, Vector3.forward) * Vector3.up * iconDistance;

            element.Action = menu.Elements[i].Action;
            _cakePieces.Add(element);
        }

        head.sprite = figure.FigureData.HeadSprite;
        body.sprite = figure.FigureData.BodySprite;

        _isBuilt = true;
    }

    private void Update()
    {
        if (_isBuilt)
        {
            float mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + _stepLength / 2f);
            int activeElementIndex = (int)(mouseAngle / _stepLength);

            for (int i = 0; i < piecesCount; i++)
            {
                if (i == activeElementIndex)
                {
                    _cakePieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.75f);

                }
                else
                {
                    _cakePieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                switch (_cakePieces[activeElementIndex].Action)
                {
                    case PossibleActions.Cancel:
                        RadialMenuManager.Instance.CloseMenu();
                        break;
                    case PossibleActions.Leave:
                        RadialMenuManager.Instance.ConfirmSendingSpriteAway();
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
