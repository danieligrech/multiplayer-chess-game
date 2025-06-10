using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChess;
using Unity.Netcode;

public class MoveBridge : MonoBehaviour
{
    private TurnManager _turnManager;

    void Start()
    {
        _turnManager = FindObjectOfType<TurnManager>();
        if (_turnManager == null) Debug.LogError("No TurnManager Found...");

        VisualPiece.VisualPieceMoved += OnVisualPieceMoved;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Debug.developerConsoleVisible = !Debug.developerConsoleVisible;
        }
    }

    void OnDestroy()
    {
        VisualPiece.VisualPieceMoved -= OnVisualPieceMoved;
    }

    private void OnVisualPieceMoved(Square fromSq, Transform pieceTransform, Transform targetSquareTransform, Piece promotionPiece = null)
    {
        int fromX = fromSq.File;
        int fromY = fromSq.Rank;

        var toSq = new Square(targetSquareTransform.name);
        int toX = toSq.File;
        int toY = toSq.Rank;

        if(!GameManager.Instance.TryGetLegalMove(fromSq, toSq, out Movement _))
        {
            var origGO = GameObject.Find(fromSq.ToString());
            if(origGO != null)
            {
                pieceTransform.parent = origGO.transform;
                pieceTransform.position = origGO.transform.position;
            }
            return;
        }

        var resetGO = GameObject.Find(fromSq.ToString());
        if(resetGO != null)
        {
            pieceTransform.parent = resetGO.transform;
            pieceTransform.position = resetGO.transform.position;
        }

        Debug.Log($"[MoveBridge] Requesting move {fromX},{fromY} → {toX},{toY}");
        _turnManager.RequestMoveServerRpc(fromX, fromY, toX, toY);
    }
}
