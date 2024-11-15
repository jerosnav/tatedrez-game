using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerInteractionHandler> m_playerInteractionHandlers = new List<PlayerInteractionHandler>();
    
    public static GameManager Instance { get; private set; }
    
    public event Action<PlayerInteractionHandler> OnPlayerActiveUpdated;

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
        yield return new WaitUntil(() => 
            m_playerInteractionHandlers.Count >= 2 
            && m_playerInteractionHandlers.TrueForAll(o => o.IsReady));
        StartGame();
    }

    public void RegisterPlayer(PlayerInteractionHandler playerInteractionHandler)
    {
        if (!m_playerInteractionHandlers.Contains(playerInteractionHandler))
        {
            m_playerInteractionHandlers.Add(playerInteractionHandler);
        }
    }

    public void StartGame()
    {
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
            m_playerInteractionHandlers[m_activePlayer].EnablePlayerControl(true);
        }
        OnPlayerActiveUpdated?.Invoke(m_activePlayer >= 0? m_playerInteractionHandlers[m_activePlayer] : null);
    }
}
