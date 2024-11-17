using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Board", fileName = "New Board", order = 1)]
    public class BoardData : ScriptableObject
    {
        [SerializeField] private Vector2Int m_gridSize = new Vector2Int(3, 3);
        [SerializeField] private Color m_lightColor = new Color(1f, 0.6f, 0f, 1f);
        [SerializeField] private Color m_darkColor = new Color(0.6f, 0.6f, 0f, 1f);

        public Vector2Int GridSize => m_gridSize;
        public int Width => m_gridSize.x;
        public int Height => m_gridSize.y;
        public Color LightColor => m_lightColor;
        public Color DarkColor => m_darkColor;
    }
}
