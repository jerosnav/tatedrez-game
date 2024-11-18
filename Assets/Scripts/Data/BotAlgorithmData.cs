using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Gameplay;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    public abstract class BotAlgorithmData : ScriptableObject
    {
        public struct PieceMove
        {
            public PieceDragHandler pieceDragHandler;
            public Vector2Int moveTo;
            public bool IsValid() => pieceDragHandler;

            public PieceMove(PieceDragHandler pieceDragHandler, Vector2Int moveTo)
            {
                this.pieceDragHandler = pieceDragHandler;
                this.moveTo = moveTo;
            }
        }

        public abstract IEnumerator CalculateBestMoveCo(BoardController boardController,
            PlayerInteractionHandler botPlayer, Action<PieceMove> onFinished);
    }
}