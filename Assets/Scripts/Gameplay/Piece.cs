using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.GridSystem;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] protected PieceData m_pieceData;
        [SerializeField] protected PlayerData m_playerData;

        public PieceData Data => m_pieceData;
        public PlayerData Owner => m_playerData;
    }
}