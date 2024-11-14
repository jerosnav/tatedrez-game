using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class BoardGrid : MonoBehaviour
    {
        [SerializeField] private BoardData m_boardData;
        [SerializeField] private Vector2Int m_gridSize = new Vector2Int(4, 4);

        [Header("Debug"), Space] [SerializeReference]
        private ICellElement[] m_cellElements;
        
        public int Width => m_gridSize.x;
        public int Height => m_gridSize.y;

        public bool GetCellElementAt(Vector2Int gridPosition, out ICellElement cellElement)
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

        public void SetBoardData(BoardData boardData)
        {
            m_boardData = boardData;
            m_gridSize = m_boardData.GridSize;

            int size = m_boardData.GridSize.x * m_boardData.GridSize.y;

            m_cellElements = new ICellElement[size];
        }
    }
}
