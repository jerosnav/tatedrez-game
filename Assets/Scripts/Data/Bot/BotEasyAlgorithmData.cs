using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Bot Easy Data", fileName = "Bot Easy Data", order = 1)]
    public class BotEasyAlgorithmData : BotAlgorithmData
    {
        public override Vector2Int ChooseNextMoveTicTacToeStage(BoardGrid boardGrid, PlayerData playerData)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2Int ChooseNextMoveChessStage(BoardGrid boardGrid, PlayerData playerData)
        {
            throw new System.NotImplementedException();
        }
    }
}