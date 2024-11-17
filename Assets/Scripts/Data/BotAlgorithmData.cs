using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    public abstract class BotAlgorithmData : ScriptableObject
    {
        public abstract Vector2Int ChooseNextMoveTicTacToeStage(BoardController boardGrid, PlayerData playerData);
        public abstract Vector2Int ChooseNextMoveChessStage(BoardController boardGrid, PlayerData playerData);
    }
}