using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Player Data", fileName = "New Player Data", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private string m_playerName = "";
        [SerializeField] private Color m_playerColor = Color.white;
        [SerializeField] private BotAlgorithmData m_botAlgorithmData;

        public Color PlayerColor => m_playerColor;
        public string PlayerName => string.IsNullOrEmpty(m_playerName) ? name : m_playerName;
        public BotAlgorithmData BotAlgorithm => m_botAlgorithmData;
    }
}