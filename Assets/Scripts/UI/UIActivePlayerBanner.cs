using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActivePlayerBanner : MonoBehaviour
{
    [Header("References"), Space] 
    [SerializeField] private TextMeshProUGUI m_yourTurnText;
    [SerializeField] private Image m_backgroundImage;

    private const string YOUR_TURN_MOVE_TO_BOARD = "Your Turn: Move a piece to the board";
    private const string YOUR_TURN_MOVE_ON_BOARD = "Your Turn: Move a piece on the board";
    
    private void Start()
    {
        GameManager.Instance.OnPlayerActiveUpdated += SetActivePlayer;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerActiveUpdated -= SetActivePlayer;
    }

    private void SetActivePlayer(PlayerInteractionHandler playerInteractionHandler)
    {
        if (playerInteractionHandler)
        {
            gameObject.SetActive(true);
            Color color = playerInteractionHandler.PlayerData.PlayerColor;
            color.a = 0.2f;
            m_backgroundImage.color = color;
            m_yourTurnText.text = playerInteractionHandler.AllPiecesOnBoard()
                ? YOUR_TURN_MOVE_ON_BOARD
                : YOUR_TURN_MOVE_TO_BOARD;
            transform.position = playerInteractionHandler.transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
