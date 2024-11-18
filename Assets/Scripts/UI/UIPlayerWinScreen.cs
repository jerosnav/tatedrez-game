using System;
using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.UI
{
    public class UIPlayerWinScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_winnerText;
        [SerializeField] private TextMeshProUGUI m_continueText;

        private const string PLAYER_WINNER_TEXT = "Player {0} Wins!!!";

        private void Awake()
        {
            GameManager.OnPlayerWin += playerData =>
            {
                gameObject.SetActive(true);
                if (playerData)
                {
                    m_winnerText.text = string.Format(PLAYER_WINNER_TEXT, playerData.PlayerName);
                    m_winnerText.color = playerData.PlayerColor;
                }
                else
                {
                    m_winnerText.text = "The Game is a Tie";
                    m_winnerText.color = Color.gray;
                }
            };
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            StartCoroutine(PressToContinueCo());
        }

        private IEnumerator PressToContinueCo()
        {
            m_continueText.enabled = false;
            GetComponent<Button>().enabled = false;
            yield return new WaitForSeconds(2f);
            m_continueText.enabled = true;
            GetComponent<Button>().enabled = true;
        }


        public void OnContinueButtonPressed()
        {
            GameManager.Instance.StartNewGame();
            gameObject.SetActive(false);
        }
    }
}