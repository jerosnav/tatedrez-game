using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Piece/Rook", fileName = "Rook", order = 1)]
    public class RookPieceData : PieceData
    {
        public override void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition,
            List<Vector2Int> validMoves)
        {
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, Vector2Int.right, validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, Vector2Int.down, validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, Vector2Int.left, validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, Vector2Int.up, validMoves);
        }

        
    }
}