using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    public abstract class PieceData : ScriptableObject
    {
        [SerializeField] private Sprite m_shape;
        public Sprite Shape => m_shape;

        public abstract void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition,
            List<Vector2Int> validMoves);
    }
}