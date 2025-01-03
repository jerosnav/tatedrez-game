using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.UI
{
    public class UIActivePlayerBanner : MonoBehaviour
    {
        [Header("References"), Space] [SerializeField]
        private TextMeshProUGUI m_yourTurnText;

        [SerializeField] private Image m_backgroundImage;

        private const string YOUR_TURN_MOVE_TO_BOARD = "Your Turn: Move a piece to the board";
        private const string YOUR_TURN_MOVE_ON_BOARD = "Your Turn: Move a piece on the board";

        private void Start()
        {
            GameManager.OnPlayerActiveUpdated += SetActivePlayer;
        }

        private void OnDestroy()
        {
            GameManager.OnPlayerActiveUpdated -= SetActivePlayer;
        }

        private void SetActivePlayer(PlayerInteractionHandler playerInteractionHandler)
        {
            if (playerInteractionHandler)
            {
                gameObject.SetActive(true);
                Color color = playerInteractionHandler.PlayerData.PlayerColor;
                color.a = 0.1f;
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
}