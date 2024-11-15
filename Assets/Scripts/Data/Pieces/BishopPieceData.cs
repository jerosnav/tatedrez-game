using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Piece/Bishop", fileName = "Bishop", order = 1)]
    public class BishopPieceData : PieceData
    {
        public override void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition,
            List<Vector2Int> validMoves)
        {
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, new Vector2Int(1, 1), validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, new Vector2Int(1, -1), validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, new Vector2Int(-1, 1), validMoves);
            AIGridHelper.PopulateWithLinearDirection(boardGrid, gridPosition, new Vector2Int(-1, -1), validMoves);
        }

        
    }
}