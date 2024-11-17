using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.GridSystem
{
    public class CellGrid
    {
        public event Action<CellGrid> OnBoardUpdatedEvent; 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IEnumerable<ICellElement> CellElements => m_cellElements;
        
        private ICellElement[] m_cellElements;
        
        public CellGrid(int width, int height)
        {
            Width = width;
            Height = height;
            m_cellElements = new ICellElement[Width * Height];
        }

        public void ClearGrid()
        {
            for (int i = 0; i < m_cellElements.Length; i++)
            {
                m_cellElements[i] = default;
            }
            OnBoardUpdatedEvent?.Invoke(this);
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
            if (!CanPlaceElement(cellElement, gridPosition)) return false;

            if (cellElement.CellGrid != this)
            {
                cellElement.CellGrid = this;
            }
            else
            {
                m_cellElements[cellElement.GridPosition.x + cellElement.GridPosition.y * Width] = default;
            }
            m_cellElements[gridPosition.x + gridPosition.y * Width] = cellElement;
            
            cellElement.GridPosition = gridPosition;
            OnBoardUpdatedEvent?.Invoke(this);
            return true;
        }
        
        public bool CanPlaceElement(ICellElement cellElement, Vector2Int gridPosition)
        {
            if(cellElement == default) return false;
            if (!IsValidGridPosition(gridPosition)) return false;
            if (!IsEmpty(gridPosition)) return false;
            if (!cellElement.IsValidPosition(gridPosition)) return false;
            
            return true;
        }
    }
}
