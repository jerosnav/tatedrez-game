using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private PieceDragHandler[] m_pieces;
    
    public bool IsReady { get; private set; }
    public PlayerData PlayerData => m_playerData;

    private void OnValidate()
    {
        UpdateVisuals();
    }

    private void Awake()
    {
        m_pieces = GetComponentsInChildren<PieceDragHandler>();
        IsReady = false;
    }

    private IEnumerator Start()
    {
        yield return null;
        GetComponentInChildren<LayoutGroup>().enabled = false;
        GameManager.Instance.RegisterPlayer(this);
        for (int i = 0; i < m_pieces.Length; i++)
        {
            m_pieces[i].SetRestartPosition();
        }
        IsReady = true;
    }

    public bool AllPiecesOnBoard()
    {
        return m_pieces.All(piece => piece.BoardGrid != null);
    }

    public void EnablePlayerControl(bool enableControl)
    {
        bool allPiecesAreOnBoard = AllPiecesOnBoard();
        for (int i = 0; i < m_pieces.Length; i++)
        {
            PieceDragHandler piece = m_pieces[i];
            if (piece.BoardGrid == null) 
            { // piece is not in the board yet
                piece.SetDraggable(enableControl);
            }
            else
            { // piece is in the board
                piece.SetDraggable(enableControl && allPiecesAreOnBoard);
            }
        }

        if (enableControl)
        {
            transform.SetAsLastSibling();
        }
    }

    public void RestartPieces(Action onFinished = null)
    {
        int waitingCount = m_pieces.Length;
        for (int i = 0; i < m_pieces.Length; i++)
        {
            m_pieces[i].MoveToRestartPosition(() =>
            {
                waitingCount--;
                if (waitingCount == 0)
                {
                    onFinished?.Invoke();
                }
            });
        }
    }

    private void UpdateVisuals()
    {
        if (!m_playerData) return;
        for (int i = 0; i < m_pieces.Length; i++)
        {
            m_pieces[i].SetColor(m_playerData.PlayerColor);
        }
    }
}
