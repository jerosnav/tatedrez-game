using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public interface ICellElement
    {
        public Vector2Int GridPosition { get; }
        public BoardGrid BoardGrid { get; }
    }
}