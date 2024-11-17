using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.GridSystem;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Helpers
{
    public static class AIBoardGridHelper
    {
        public static void PopulateWithValidPositionsWithLinearDirection(BoardController boardController, Vector2Int startPosition, Vector2Int direction,
                    List<Vector2Int> validMoves)
        {
            Vector2Int gridPosition = startPosition + direction;
            while (CanBeMovedTo(boardController, gridPosition))
            {
                validMoves.Add(gridPosition);
                gridPosition += direction;
            }
        }
        
        public static void PopulateWithValidPositionsWithPositionArray(BoardController boardController, Vector2Int startPosition, Vector2Int[] checkPositions,
            List<Vector2Int> validMoves)
        {
            for (int i = 0; i < checkPositions.Length; i++)
            {
                Vector2Int gridPosition = startPosition + checkPositions[i];
                if (CanBeMovedTo(boardController, gridPosition))
                {
                    validMoves.Add(gridPosition);
                }
            }
        }

        public static void PopulateWithAllFreePositions(BoardController boardController, List<Vector2Int> validMoves)
        {
            Vector2Int gridPosition = Vector2Int.zero;
            for (gridPosition.y = 0; gridPosition.y < boardController.Height; gridPosition.y++)
            {
                for (gridPosition.x = 0; gridPosition.x < boardController.Width; gridPosition.x++)
                {
                    if (boardController.IsEmptyPosition(gridPosition))
                    {
                        validMoves.Add(gridPosition);
                    }
                }
            }
        }

        public static bool CanBeMovedTo(BoardController boardController, Vector2Int gridPosition)
        {
            return (boardController.TryGetPieceAt(gridPosition,
                        out Piece piece) // grid position inside board 
                    && piece == default); // is empty cell
        }

        public static bool CheckWinCondition(BoardController boardController, out PlayerData winner)
        {
            // Check Rows
            for (Vector2Int gridPosition = Vector2Int.zero; gridPosition.y < boardController.Height; gridPosition.y++)
            {
                if (IsWinningLine(boardController, gridPosition, Vector2Int.right, out winner)) return true;
            }
            // Check Columns
            for (Vector2Int gridPosition = Vector2Int.zero; gridPosition.x < boardController.Width; gridPosition.x++)
            {
                if (IsWinningLine(boardController, gridPosition, Vector2Int.up, out winner)) return true;
            }
            // Check Diagonals
            if (IsWinningLine(boardController, Vector2Int.zero, new Vector2Int(1, 1), out winner)) return true;
            if (IsWinningLine(boardController, new Vector2Int(0, boardController.Height - 1), new Vector2Int(1, -1), out winner)) return true;
            
            return false;
        }

        private static bool IsWinningLine(BoardController boardController, Vector2Int startPosition, Vector2Int stepIncrement, out PlayerData winner)
        {
            winner = default;
            Vector2Int gridPosition = startPosition;
            if (!boardController.TryGetPieceAt(gridPosition, out Piece piece) || piece == default)
            {
                return false;
            }
            Piece valueToCheck = piece.Value as Piece;
            if (!valueToCheck)
            {
                return false;
            }

            gridPosition += stepIncrement;
            while (boardController.TryGetPieceAt(gridPosition, out piece))
            {
                if (!piece || piece != valueToCheck)
                {
                    return false;
                }
                gridPosition += stepIncrement;
            }

            winner = valueToCheck.Owner;
            return true;
        }
    }
}