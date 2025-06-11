using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using UnityChess;
using System;

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> WhiteClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> BlackClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> IsWhiteTurn = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake() => DontDestroyOnLoad(gameObject);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            WhiteClientId.Value = NetworkManager.LocalClientId;
            NetworkManager.OnClientConnectedCallback += AssignBlack;
        }

        IsWhiteTurn.OnValueChanged += (prev, curr) => Debug.Log($"It is now {(curr ? "White" : "Black")}'s turn!");
    }

    private void AssignBlack(ulong clientId)
    {
        if (clientId == WhiteClientId.Value || BlackClientId.Value != 0) return;
        BlackClientId.Value = clientId;
        Debug.Log($"Client {clientId} is now the Black Player!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(int fromX, int fromY, int toX, int toY, ServerRpcParams rpcParams = default)
    {
        ulong sender = rpcParams.Receive.SenderClientId;
        bool isSenderWhite = sender == WhiteClientId.Value;

        Debug.Log($"[ServerRpc] Move request from {rpcParams.Receive.SenderClientId}: {fromX},{fromY} → {toX},{toY}");

        if (isSenderWhite != IsWhiteTurn.Value) return;

        var fromSq = new Square(fromX, fromY);
        var toSq = new Square(toX, toY);

        if (!GameManager.Instance.TryGetLegalMove(fromSq, toSq, out Movement move))
            return;

        if (!GameManager.Instance.ExecuteMove(move))
            return;

        GameManager.Instance.HalfMoveTimeline.TryGetCurrent(out HalfMove latest);
        if (latest.CausedCheckmate)
        {
            string winner = isSenderWhite ? "White" : "Black";
            EndGameClientRpc($"{winner} has Won by Checkmate!");
            return;
        }
        if (latest.CausedStalemate)
        {
            EndGameClientRpc("Game Drawn by Stalemate...");
            return;
        }

        IsWhiteTurn.Value = !IsWhiteTurn.Value;
        MovePieceClientRpc(fromX, fromY, toX, toY);
    }

    [ClientRpc]
    void MovePieceClientRpc(int fromX, int fromY, int toX, int toY, ClientRpcParams rpcParams = default)
    {
        var fromSq = new UnityChess.Square(fromX, fromY);
        var toSq = new UnityChess.Square(toX, toY);

        if(GameManager.Instance.TryGetLegalMove(fromSq, toSq, out var move))
        {
            GameManager.Instance.ExecuteMove(move);
        }
        else
        {
            Debug.LogWarning($"Client Has Attempted an Invalid Move..: {fromX}, {fromY} to {toX}, {toY}");
        }

        BoardManager.Instance.RefreshBoardVisuals();
    }

    [ClientRpc]
    void EndGameClientRpc(string resultMessage)
    {
        Debug.Log($"[EndGameClientRpc] {resultMessage}");
        UIManager.Instance.ShowGameEnd(resultMessage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResignServerRpc(ServerRpcParams rpcParams = default)
    {
        var resigner = rpcParams.Receive.SenderClientId;
        var winner = resigner == WhiteClientId.Value ? BlackClientId.Value : WhiteClientId.Value;

        string winningSide = winner == WhiteClientId.Value ? "White" : "Black";
        EndGameClientRpc($"{winningSide} Has Won Via Resignation!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void PingServerRpc(long clientTimestamp, ServerRpcParams rpcParams = default)
    {
        PongClientRpc(clientTimestamp, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void PongClientRpc(long clientTimestamp, ulong clientId, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long rtt = now - clientTimestamp;

        Debug.Log($"[Latency] RTT = {rtt}ms");
        UIManager.Instance.ShowLatency(rtt);
    }
}
