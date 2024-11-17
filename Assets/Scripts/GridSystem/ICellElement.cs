using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.GridSystem
{
    public interface ICellElement<TValue>
    {
        public Vector2Int GridPosition { get; set; }
        /// <summary>
        /// This is the owner Cell Grid where the element is placed. If null, GridPosition is meanness
        /// </summary>
        public CellGrid<TValue> CellGrid { get; set; }
        
        // This is used to custom element validation. It is not considering if the gridPosition is occupied, 
        // only if the movement to that position is valid
        public bool IsValidPosition(Vector2Int gridPosition);
        // This could be anything derived from UnityEngine.Object, like a monobehaviour, sprite, etc
        // It is holding the actual object connected with this cell element
        public TValue Value { get; }
    }
}