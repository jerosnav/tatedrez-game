using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Piece/Bishop", fileName = "Bishop", order = 1)]
    public class BishopPieceData : PieceData
    {
        public override void PopulateWithValidMoves(BoardController boardController, Vector2Int gridPosition,
            List<Vector2Int> validMoves)
        {
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, new Vector2Int(1, 1), validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, new Vector2Int(1, -1), validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, new Vector2Int(-1, 1), validMoves);
            AIBoardGridHelper.PopulateWithValidPositionsWithLinearDirection(boardController, gridPosition, new Vector2Int(-1, -1), validMoves);
        }

        
    }
}