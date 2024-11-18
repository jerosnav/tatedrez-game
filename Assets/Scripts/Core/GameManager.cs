using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AwesomeCompany.Tatedrez.Core
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardController m_boardController;
        [SerializeField] private List<PlayerInteractionHandler> m_playerInteractionHandlers 
            = new List<PlayerInteractionHandler>();

        public static GameManager Instance { get; private set; }
        public BoardController BoardController => m_boardController;

        public static event Action<PlayerInteractionHandler> OnPlayerActiveUpdated;
        public static event Action<PlayerData> OnPlayerWin;

        private int m_activePlayer = -1;
        

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
            yield return new WaitUntil(() => m_boardController);
            m_boardController.OnBoardUpdatedEvent += BoardGridOnOnBoardUpdatedEvent;
            yield return new WaitUntil(() =>
                m_playerInteractionHandlers.Count >= 2
                && m_playerInteractionHandlers.TrueForAll(o => o.IsReady));
            StartNewGame();
        }

        private void BoardGridOnOnBoardUpdatedEvent(BoardController boardController)
        {
            if ( boardController.CheckWinCondition(out PlayerData winner))
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

        public void RegisterBoardController(BoardController boardController)
        {
            m_boardController = boardController;
        }

        public void StartNewGame()
        {
            m_boardController.ClearBoard();
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
                var activePlayer = m_playerInteractionHandlers[m_activePlayer];
                if (activePlayer.CanMoveAnyPiece())
                {
                    if (activePlayer.PlayerData.BotAlgorithm)
                    {
                        StartCoroutine(PlayBotMovementCo());
                    }

                    activePlayer.EnablePlayerControl(true);
                }
                else
                {
                    StartCoroutine(DelayedEndTurn(1f));
                }
            }

            OnPlayerActiveUpdated?.Invoke(m_activePlayer >= 0 ? m_playerInteractionHandlers[m_activePlayer] : null);
        }

        IEnumerator DelayedEndTurn(float time)
        {
            yield return new WaitForSeconds(time);
            EndPlayerTurn();
        }
        
        IEnumerator PlayBotMovementCo()
        {
            PlayerInteractionHandler botPlayer = m_playerInteractionHandlers[m_activePlayer];
            BotAlgorithmData.PieceMove bestPieceMove = default;
            yield return botPlayer.PlayerData.BotAlgorithm.CalculateBestMoveCo(m_boardController, botPlayer, pieceMove =>
            {
                bestPieceMove = pieceMove;
            });
            
            yield return new WaitForSeconds(1f);
            
            if (bestPieceMove.IsValid())
            {
                bestPieceMove.pieceDragHandler.MoveTo(m_boardController, bestPieceMove.moveTo, EndPlayerTurn);
            }
            else
            {
                OnPlayerWin?.Invoke(null);
            }
            
        }
    }
}