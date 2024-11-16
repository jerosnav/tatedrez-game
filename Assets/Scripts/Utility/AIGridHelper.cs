using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Helpers
{
    public static class AIGridHelper
    {
        public static void PopulateWithValidPositionsWithLinearDirection(BoardGrid boardGrid, Vector2Int startPosition, Vector2Int direction,
                    List<Vector2Int> validMoves)
        {
            Vector2Int gridPosition = startPosition + direction;
            while (CanBeMovedTo(boardGrid, gridPosition))
            {
                validMoves.Add(gridPosition);
                gridPosition += direction;
            }
        }
        
        public static void PopulateWithValidPositionsWithPositionArray(BoardGrid boardGrid, Vector2Int startPosition, Vector2Int[] checkPositions,
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

        public static void PopulateWithAllFreePositions(BoardGrid boardGrid, List<Vector2Int> validMoves)
        {
            Vector2Int gridPosition = Vector2Int.zero;
            for (gridPosition.y = 0; gridPosition.y < boardGrid.Height; gridPosition.y++)
            {
                for (gridPosition.x = 0; gridPosition.x < boardGrid.Width; gridPosition.x++)
                {
                    if (boardGrid.IsEmpty(gridPosition))
                    {
                        validMoves.Add(gridPosition);
                    }
                }
            }
        }

        public static bool CanBeMovedTo(BoardGrid boardGrid, Vector2Int gridPosition)
        {
            return (boardGrid.TryGetCellElementAt(gridPosition,
                        out ICellElement cellElement) // grid position inside board 
                    && cellElement == default); // is empty cell
        }

        public static bool CheckWinCondition(BoardGrid boardGrid, out PlayerData winner)
        {
            // Check Rows
            for (Vector2Int gridPosition = Vector2Int.zero; gridPosition.y < boardGrid.Height; gridPosition.y++)
            {
                if (IsWinningLine(boardGrid, gridPosition, Vector2Int.right, out winner)) return true;
            }
            // Check Columns
            for (Vector2Int gridPosition = Vector2Int.zero; gridPosition.x < boardGrid.Width; gridPosition.x++)
            {
                if (IsWinningLine(boardGrid, gridPosition, Vector2Int.up, out winner)) return true;
            }
            // Check Diagonals
            if (IsWinningLine(boardGrid, Vector2Int.zero, new Vector2Int(1, 1), out winner)) return true;
            if (IsWinningLine(boardGrid, new Vector2Int(0, boardGrid.Height - 1), new Vector2Int(1, -1), out winner)) return true;
            
            return false;
        }

        private static bool IsWinningLine(BoardGrid boardGrid, Vector2Int startPosition, Vector2Int stepIncrement, out PlayerData winner)
        {
            winner = default;
            Vector2Int gridPosition = startPosition;
            if (!boardGrid.TryGetCellElementAt(gridPosition, out ICellElement cellElement) || cellElement == default)
            {
                return false;
            }
            PlayerData valueToCheck = cellElement.Value as PlayerData;
            if (valueToCheck == null)
            {
                return false;
            }

            gridPosition += stepIncrement;
            while (boardGrid.TryGetCellElementAt(gridPosition, out cellElement))
            {
                if (cellElement == default || cellElement.Value != valueToCheck)
                {
                    return false;
                }
                gridPosition += stepIncrement;
            }

            winner = valueToCheck;
            return true;
        }
    }
}