using AwesomeCompany.Tatedrez.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.UI
{
    public class UICell : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Vector2Int m_gridPosition;
        [SerializeField] private Image m_image;
        [SerializeField] private UIBoardWidget m_uiBoardWidget;

        public Vector2Int GridPositions => m_gridPosition;

        public void Setup(UIBoardWidget uiBoardWidget, Vector2Int gridPosition, Color color)
        {
            m_uiBoardWidget = uiBoardWidget;
            m_gridPosition = gridPosition;
            m_image.color = color;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag
                && eventData.pointerDrag.TryGetComponent(out ICellElement cellElement))
            {
                if (GameManager.Instance.BoardGrid.TryPlaceElement(cellElement, m_gridPosition))
                {
                    eventData.pointerDrag.transform.position = transform.position;
                    eventData.Use();
                    GameManager.Instance.EndPlayerTurn();
                }
            }
        }
    }
}