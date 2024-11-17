using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Piece/Rook", fileName = "Rook", order = 1)]
    public class RookPieceData : PieceData
    {
        public override void PopulateWithValidMoves(BoardController boardController, Vector2Int gridPosition,
            List<Vector2Int> validMoves)
        {
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, Vector2Int.right, validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, Vector2Int.down, validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, Vector2Int.left, validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, Vector2Int.up, validMoves);
        }

        
    }
}