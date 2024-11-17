using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.GridSystem;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwesomeCompany.Tatedrez.Core
{
    public class BoardController : MonoBehaviour
    {
        public event Action<BoardController> OnBoardUpdatedEvent;
        
        [SerializeField] private BoardData m_boardData;

        [Header("References"), Space] 
        [SerializeField] private GridLayoutGroup m_gridLayoutGroup;
        [SerializeField] private Image m_cellPrefab;
        [SerializeField] private BoardCell[] m_uiCells;
        

        private CellGrid m_cellGrid;
        private HashSet<PieceCellElement> m_cellElementOnBoard = new HashSet<PieceCellElement>();

        public int Width => m_cellGrid.Width;
        public int Height => m_cellGrid.Height;

#if UNITY_EDITOR        
        private void OnValidate()
        {
            if (gameObject.scene.IsValid())
            {   // delayCall is reset by unity so there is no need to remove the callback
                // The if UNITY_EDITOR define allows to call UnityEditor without any issue when making the build
                // This way of calling updating methods is safer than using OnValidate itself (when saving prefabs, or when unity calls validate when importing assets)
                UnityEditor.EditorApplication.delayCall += UpdateVisuals;
            }
        }
#endif

        private void Awake()
        {
            m_cellGrid = new CellGrid(m_boardData.Width, m_boardData.Height);
            m_cellGrid.OnBoardUpdatedEvent += (_) => OnBoardUpdatedEvent?.Invoke(this);
        }

        private void Start()
        {
            GameManager.Instance.RegisterBoardController(this);
        }

        public void ClearBoard()
        {
            m_cellElementOnBoard.Clear();
            m_cellGrid.ClearGrid();
        }

        public bool IsEmptyPosition(Vector2Int gridPosition)
        {
            return m_cellGrid.IsEmpty(gridPosition);
        }

        public bool TryGetPieceAt(Vector2Int gridPosition, out Piece piece)
        {
            piece = null;
            if (!m_cellGrid.TryGetCellElementAt(gridPosition, out ICellElement cellElement)) return false;
            piece = ((PieceCellElement)cellElement)?.Piece;
            return true;
        }

        public bool TryToPlacePiece(Piece piece, Vector2Int gridPosition)
        {
            PieceCellElement pieceCellElement = GetOrCreateCellElement(piece);
            return m_cellGrid.TryPlaceElement(pieceCellElement, gridPosition);
        }

        public bool CanPlacePiece(Piece piece, Vector2Int gridPosition)
        {
            var cellElement = m_cellElementOnBoard.FirstOrDefault(o => o.Piece == piece);
            return cellElement != default && m_cellGrid.CanPlaceElement(cellElement, gridPosition);
        }

        private PieceCellElement GetOrCreateCellElement(Piece piece)
        {
            var pieceCellElement =  m_cellElementOnBoard.FirstOrDefault(o => o.Piece == piece);
            if (pieceCellElement != null) return pieceCellElement;

            pieceCellElement = new PieceCellElement(piece, this);
            m_cellElementOnBoard.Add(pieceCellElement);
            return pieceCellElement;
        }

        public bool CheckWinCondition(out PlayerData playerData)
        {
            return AIBoardGridHelper.CheckWinCondition(this, out playerData);
        }

        public void UpdateVisuals()
        {
            if (!m_boardData) return;
            if (!m_gridLayoutGroup) return;
            
            m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            m_gridLayoutGroup.constraintCount = m_boardData.GridSize.x;

            int size = m_boardData.GridSize.x * m_boardData.GridSize.y;
            
            // Add cells if needed
            int neededCells = size - m_gridLayoutGroup.transform.childCount;
            for (int i = 0; i < neededCells; i++)
            {
                Instantiate(m_cellPrefab, m_gridLayoutGroup.transform);
            }            

            // Enable or disable cells as needed (do not remove them) to avoid memory fragmentation by reusing cells.
            for (int i = 0; i < m_gridLayoutGroup.transform.childCount; i++)
            {
                m_gridLayoutGroup.transform.GetChild(i).gameObject.SetActive(i < size);
            }

            m_uiCells = m_gridLayoutGroup.GetComponentsInChildren<BoardCell>();

            int boardWidth = m_boardData.GridSize.x;
            for (int i = 0; i < m_uiCells.Length; i++)
            {
                int row = i % boardWidth;
                int col = i / boardWidth;
                bool check = (row + col) % 2 == 0;
                Color color = check ? m_boardData.DarkColor : m_boardData.LightColor;
                Vector2Int gridPosition = new Vector2Int(row, col);
                m_uiCells[i].Setup(this, gridPosition, color);
            }
        }
        
        private class PieceCellElement : ICellElement
        {
            public Vector2Int GridPosition { get; set; }
            public CellGrid CellGrid { get; set; }
            public bool IsValidPosition(Vector2Int gridPosition)
            {
                // If cell grid is null, the piece is still to be placed on board, so any free position is valid
                if (CellGrid == null) return true;
            
                // Populate with all the valid moves for the current piece data assigned
                Piece.Data.PopulateWithValidMoves(m_boardController, GridPosition, m_reusableValidPositions);
                return m_reusableValidPositions.Count == 0 || m_reusableValidPositions.Contains(gridPosition);
            }

            public Object Value { get; }
            public Piece Piece => Value as Piece;

            private BoardController m_boardController;

            public PieceCellElement(Piece piece, BoardController boardController)
            {
                Value = piece;
                m_boardController = boardController;
            }
            
            private List<Vector2Int> m_reusableValidPositions = new List<Vector2Int>();
        }
    }
}