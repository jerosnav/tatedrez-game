using System;
using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private PieceDragHandler[] m_pieces;
    
    public bool IsReady { get; private set; }

    private void Awake()
    {
        m_pieces = GetComponentsInChildren<PieceDragHandler>();
        IsReady = false;
    }

    private IEnumerator Start()
    {
        yield return null;
        GetComponent<LayoutGroup>().enabled = false;
        GameManager.Instance.RegisterPlayer(this);
        for (int i = 0; i < m_pieces.Length; i++)
        {
            m_pieces[i].SetRestartPosition();
        }
        IsReady = true;
    }

    public void EnablePlayerControl(bool enableControl)
    {
        for (int i = 0; i < m_pieces.Length; i++)
        {
            m_pieces[i].SetDraggable(enableControl);
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
}
