using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class PlayerInteractionHandler : MonoBehaviour
    {
        [SerializeField] private PlayerData m_playerData;
        [SerializeField] private PieceDragHandler[] m_pieces;
        [SerializeField] private TextMeshProUGUI m_playerNameText;

        public bool IsReady { get; private set; }
        public PlayerData PlayerData => m_playerData;
        public IEnumerable<PieceDragHandler> Pieces => m_pieces;

        private List<Vector2Int> m_reusablePossibleMoves = new List<Vector2Int>();

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
                PieceDragHandler pieceDragHandler = m_pieces[i];
                pieceDragHandler.SetRestartPosition();
                pieceDragHandler.SetPlayerData(m_playerData);
            }

            if (m_playerData.BotAlgorithm)
            {
                EnablePlayerControl(false);
            }

            IsReady = true;
        }

        public bool AllPiecesOnBoard()
        {
            return m_pieces.All(piece => piece.IsPlacedOnBoard);
        }

        public bool CanMoveAnyPiece()
        {
            if (!AllPiecesOnBoard()) return true;
            for (int i = 0; i < m_pieces.Length; i++)
            {
                m_reusablePossibleMoves.Clear();
                PieceDragHandler pieceDragHandler = m_pieces[i];
                pieceDragHandler.Data.PopulateWithValidMoves(GameManager.Instance.BoardController, pieceDragHandler.GridPosition, m_reusablePossibleMoves);
                if (m_reusablePossibleMoves.Count > 0) return true;
            }

            return false;
        }

        public void EnablePlayerControl(bool enableControl)
        {
            if (enableControl)
            {
                transform.SetAsLastSibling();
            }
            if (m_playerData.BotAlgorithm && enableControl) return;
            
            bool allPiecesAreOnBoard = AllPiecesOnBoard();
            for (int i = 0; i < m_pieces.Length; i++)
            {
                PieceDragHandler piece = m_pieces[i];
                if (!piece.IsPlacedOnBoard)
                {
                    // piece is not in the board yet
                    piece.SetDraggable(enableControl);
                }
                else
                {
                    // piece is in the board
                    piece.SetDraggable(enableControl && allPiecesAreOnBoard);
                }
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

            m_playerNameText.text = m_playerData.PlayerName;
        }
    }
}