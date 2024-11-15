using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class BoardGrid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private ICellElement[] m_cellElements;
        
        public BoardGrid(int width, int height)
        {
            Width = width;
            Height = height;
            m_cellElements = new ICellElement[Width * Height];
        }
        

        public bool TryGetCellElementAt(Vector2Int gridPosition, out ICellElement cellElement)
        {
            cellElement = default;
            if (!IsValidGridPosition(gridPosition)) return false;
            cellElement = m_cellElements[gridPosition.x + gridPosition.y * Width];
            return true;
        }

        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < Width
                                       && gridPosition.y >= 0 && gridPosition.y < Height;
        }

        public bool IsEmpty(Vector2Int gridPosition)
        {
            return IsValidGridPosition(gridPosition) &&
                   m_cellElements[gridPosition.x + gridPosition.y * Width] == default;
        }

        public bool TryPlaceElement(ICellElement cellElement, Vector2Int gridPosition)
        {
            if (!IsValidGridPosition(gridPosition)) return false;
            Debug.Log("Piece on " + gridPosition + ": " + (PieceDragHandler)m_cellElements[gridPosition.x + gridPosition.y * Width], (PieceDragHandler)m_cellElements[gridPosition.x + gridPosition.y * Width]);
            if (!IsEmpty(gridPosition)) return false;

            m_cellElements[cellElement.GridPosition.x + cellElement.GridPosition.y * Width] = default;
            m_cellElements[gridPosition.x + gridPosition.y * Width] = cellElement;
            cellElement.BoardGrid = this;
            cellElement.GridPosition = gridPosition;
            return true;
        }
    }
}
