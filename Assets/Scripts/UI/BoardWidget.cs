using System;
using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.UI
{
    public class BoardWidget : MonoBehaviour
    {
        [SerializeField] private BoardData m_boardData;

        [Header("References"), Space] 
        [SerializeField] private GridLayoutGroup m_gridLayoutGroup;
        [SerializeField] private Image m_cellPrefab;
        [SerializeField] private UICell[] m_uiCells;
        
        private BoardGrid m_boardGrid;
        
        public BoardGrid BoardGrid => m_boardGrid;

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

        private void Start()
        {
            m_boardGrid = new BoardGrid(m_boardData.GridSize.x, m_boardData.GridSize.y);
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateVisuals();
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

            m_uiCells = m_gridLayoutGroup.GetComponentsInChildren<UICell>();

            int boardWidth = m_boardData.GridSize.x;
            for (int i = 0; i < m_uiCells.Length; i++)
            {
                int row = i / boardWidth;
                int col = i % boardWidth;
                bool check = (row + col) % 2 == 0;
                Color color = check ? m_boardData.DarkColor : m_boardData.LightColor;
                Vector2Int gridPosition = new Vector2Int(row, col);
                m_uiCells[i].Setup(this, gridPosition, color);
            }
        }
    }
}