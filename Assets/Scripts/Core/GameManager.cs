using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AwesomeCompany.Tatedrez.Core
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardData m_boardData;
        
        [SerializeField] private List<PlayerInteractionHandler> m_playerInteractionHandlers 
            = new List<PlayerInteractionHandler>();

        public static GameManager Instance { get; private set; }

        public static event Action<PlayerInteractionHandler> OnPlayerActiveUpdated;
        public static event Action<PlayerData> OnPlayerWin;

        private int m_activePlayer = -1;

        public BoardGrid BoardGrid { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private IEnumerator Start()
        {
            BoardGrid = new BoardGrid(m_boardData.GridSize.x, m_boardData.GridSize.y);
            BoardGrid.OnBoardUpdatedEvent += BoardGridOnOnBoardUpdatedEvent;
            yield return new WaitUntil(() =>
                m_playerInteractionHandlers.Count >= 2
                && m_playerInteractionHandlers.TrueForAll(o => o.IsReady));
            StartNewGame();
        }

        private void BoardGridOnOnBoardUpdatedEvent(BoardGrid boardGrid)
        {
            if (AIGridHelper.CheckWinCondition(boardGrid, out PlayerData winner))
            {
                OnPlayerWin?.Invoke(winner);
                SetActivePlayer(-1);
            }
        }

        public void RegisterPlayer(PlayerInteractionHandler playerInteractionHandler)
        {
            if (!m_playerInteractionHandlers.Contains(playerInteractionHandler))
            {
                m_playerInteractionHandlers.Add(playerInteractionHandler);
            }
        }

        public void StartNewGame()
        {
            BoardGrid.ClearBoard();
            SetActivePlayer(-1);
            int waitingCount = m_playerInteractionHandlers.Count;
            for (int i = 0; i < m_playerInteractionHandlers.Count; i++)
            {
                PlayerInteractionHandler handler = m_playerInteractionHandlers[i];
                handler.EnablePlayerControl(false);
                handler.RestartPieces(() =>
                {
                    waitingCount--;
                    if (waitingCount == 0)
                    {
                        // Select a random player to start
                        SetActivePlayer(Random.Range(0, m_playerInteractionHandlers.Count));
                    }
                });
            }
        }

        public void EndPlayerTurn()
        {
            int nextPlayer = (m_activePlayer + 1) % m_playerInteractionHandlers.Count;
            SetActivePlayer(nextPlayer);
        }

        public void SetActivePlayer(int playerIdx)
        {
            if (m_activePlayer >= 0)
            {
                m_playerInteractionHandlers[m_activePlayer].EnablePlayerControl(false);
            }

            m_activePlayer = playerIdx;

            if (m_activePlayer >= 0)
            {
                if (m_playerInteractionHandlers[m_activePlayer].PlayerData.BotAlgorithm)
                {
                    StartCoroutine(PlayBotMovementCo());
                }
                else
                {
                    m_playerInteractionHandlers[m_activePlayer].EnablePlayerControl(true);
                }
            }

            OnPlayerActiveUpdated?.Invoke(m_activePlayer >= 0 ? m_playerInteractionHandlers[m_activePlayer] : null);
        }

        private Dictionary<PieceDragHandler, List<Vector2Int>> m_reusablePossibleMovesDic =
            new Dictionary<PieceDragHandler, List<Vector2Int>>();

        private List<KeyValuePair<PieceDragHandler, Vector2Int>> m_reusableFlattenPossibleMovesByPiece =
            new List<KeyValuePair<PieceDragHandler, Vector2Int>>();
        IEnumerator PlayBotMovementCo()
        {
            PlayerInteractionHandler botPlayer = m_playerInteractionHandlers[m_activePlayer];
            bool allPiecesOnBoard = botPlayer.AllPiecesOnBoard();
            foreach (var pieceDragHandler in botPlayer.Pieces)
            {
                if (!m_reusablePossibleMovesDic.TryGetValue(pieceDragHandler,
                        out List<Vector2Int> possibleMovesByPiece))
                {
                    possibleMovesByPiece = m_reusablePossibleMovesDic[pieceDragHandler] = new List<Vector2Int>();
                }
                possibleMovesByPiece.Clear();
                if (allPiecesOnBoard)
                {
                    pieceDragHandler.PieceData.PopulateWithValidMoves(BoardGrid, pieceDragHandler.GridPosition, possibleMovesByPiece);
                }
                else if(pieceDragHandler.BoardGrid == null) // take only pieces not on board
                {
                    AIGridHelper.PopulateWithAllFreePositions(BoardGrid, possibleMovesByPiece);
                }
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            m_reusableFlattenPossibleMovesByPiece.Clear();
            m_reusableFlattenPossibleMovesByPiece.AddRange(m_reusablePossibleMovesDic
                .SelectMany(kvp =>
                    kvp.Value.Select(move => new KeyValuePair<PieceDragHandler, Vector2Int>(kvp.Key, move))));

            Debug.Log("flatten " + m_reusableFlattenPossibleMovesByPiece.Count);
            foreach (var kvp in m_reusablePossibleMovesDic)
            {
                Debug.Log(kvp.Key + " " + kvp.Value.Count);
            }
            
            // Check if no movement is possible and in that case ends with a tie.
            if (m_reusableFlattenPossibleMovesByPiece.Count == 0)
            {
                OnPlayerWin?.Invoke(null);
                yield break;
            }
            
            // Just get a random valid movement
            int randIdx = Random.Range(0, m_reusableFlattenPossibleMovesByPiece.Count);
            var randMove = m_reusableFlattenPossibleMovesByPiece[randIdx];
            randMove.Key.MoveTo(BoardGrid, randMove.Value, EndPlayerTurn);
        }
    }
}