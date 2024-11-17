using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        

        private CellGrid<Piece> m_cellGrid;
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
            m_cellGrid = new CellGrid<Piece>(m_boardData.Width, m_boardData.Height);
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
        
        public bool IsPieceOnBoard(Piece piece)
        {
            return m_cellElementOnBoard.Any(o => o.Value == piece);
        }

        public bool IsEmptyPosition(Vector2Int gridPosition)
        {
            return m_cellGrid.IsEmpty(gridPosition);
        }

        public bool TryGetPieceAt(Vector2Int gridPosition, out Piece piece)
        {
            piece = null;
            if (!m_cellGrid.TryGetCellElementAt(gridPosition, out ICellElement<Piece> cellElement)) return false;
            piece = ((PieceCellElement)cellElement)?.Value;
            return true;
        }

        public bool TryToPlacePiece(Piece piece, Vector2Int gridPosition)
        {
            PieceCellElement pieceCellElement = GetOrCreateCellElement(piece);
            return m_cellGrid.TryPlaceElement(pieceCellElement, gridPosition);
        }

        public bool TryGetGridPosition(Piece piece, out Vector2Int gridPosition)
        {
            var pieceCellElement = m_cellElementOnBoard.FirstOrDefault(o => o.Value == piece);
            if (pieceCellElement != null)
            {
                gridPosition = pieceCellElement.GridPosition;
                return true;
            }

            gridPosition = default;
            return false;
        }
        
        public bool TryGetWorldPosition(Vector2Int gridPosition, out Vector3 worldPosition)
        {
            var cell = m_uiCells.FirstOrDefault(cell => cell.GridPosition == gridPosition);
            if (cell)
            {
                worldPosition = cell.transform.position;
                return true;
            }

            worldPosition = default;
            return false;
        }

        public bool CanPlacePiece(Piece piece, Vector2Int gridPosition)
        {
            var cellElement = m_cellElementOnBoard.FirstOrDefault(o => o.Value == piece);
            return m_cellGrid.CanPlaceElement(cellElement, gridPosition);
        }

        private PieceCellElement GetOrCreateCellElement(Piece piece)
        {
            var pieceCellElement =  m_cellElementOnBoard.FirstOrDefault(o => o.Value == piece);
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

        public void DebugBoardData()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < m_cellGrid.Height; y++)
            {
                for (int x = 0; x < m_cellGrid.Width; x++)
                {
                    if(TryGetPieceAt(new Vector2Int(x, y), out Piece piece) && piece)
                    {
                        sb.Append(piece.Data.name + " " + piece.Owner.PlayerName + ", ");
                    }
                    else
                    {
                        sb.Append("<empty>, ");
                    }
                }
                sb.AppendLine("");
            }
            Debug.Log(sb.ToString());
        }
        
        private class PieceCellElement : ICellElement<Piece>
        {
            public Vector2Int GridPosition { get; set; }
            public CellGrid<Piece> CellGrid { get; set; }
            public Piece Value { get; }
            
            public PieceCellElement(Piece piece, BoardController boardController)
            {
                Value = piece;
                m_boardController = boardController;
            }

            private BoardController m_boardController;
            private List<Vector2Int> m_reusableValidPositions = new List<Vector2Int>();
            
            public bool IsValidPosition(Vector2Int gridPosition)
            {
                // If cell grid is null, the piece is still to be placed on board, so any free position is valid
                if (CellGrid == null) return true;
            
                // Populate with all the valid moves for the current piece data assigned
                m_reusableValidPositions.Clear();
                Value.Data.PopulateWithValidMoves(m_boardController, GridPosition, m_reusableValidPositions);
                StringBuilder sb = new StringBuilder(Value.name);
                foreach (var validPosition in m_reusableValidPositions)
                {
                    sb.Append(validPosition + ", ");
                }
                Debug.Log(sb.ToString());
                return m_reusableValidPositions.Contains(gridPosition);
            }
        }
    }
}