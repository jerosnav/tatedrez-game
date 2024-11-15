using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Gameplay;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Helpers
{
    public static class AIGridHelper
    {
        public static void PopulateWithLinearDirection(BoardGrid boardGrid, Vector2Int startPosition, Vector2Int direction,
                    List<Vector2Int> validMoves)
        {
            Vector2Int gridPosition = startPosition + direction;
            while (CanBeMovedTo(boardGrid, gridPosition))
            {
                validMoves.Add(gridPosition);
                gridPosition = startPosition + direction;
            }
        }
        
        public static void PopulateWithPositions(BoardGrid boardGrid, Vector2Int startPosition, Vector2Int[] checkPositions,
            List<Vector2Int> validMoves)
        {
            for (int i = 0; i < checkPositions.Length; i++)
            {
                Vector2Int gridPosition = startPosition + checkPositions[i];
                if (CanBeMovedTo(boardGrid, gridPosition))
                {
                    validMoves.Add(gridPosition);
                }
            }
        }

        public static bool CanBeMovedTo(BoardGrid boardGrid, Vector2Int gridPosition)
        {
            return (boardGrid.TryGetCellElementAt(gridPosition,
                        out ICellElement cellElement) // grid position inside board 
                    && cellElement == default); // is empty cell
        }
    }
}