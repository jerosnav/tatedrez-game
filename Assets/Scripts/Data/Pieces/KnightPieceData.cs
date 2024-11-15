using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Piece/Knight", fileName = "Knight", order = 1)]
    public class KnightPieceData : PieceData
    {
        private static readonly Vector2Int[] s_checkPositions = new Vector2Int[]
        {
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
        };
        
        public override void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition,
            List<Vector2Int> validMoves)
        {
            
            AIGridHelper.PopulateWithPositions(boardGrid, gridPosition, s_checkPositions, validMoves);
        }
    }
}