using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AwesomeCompany.Tatedrez.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Bot Easy Data", fileName = "Bot Easy Data", order = 1)]
    public class BotEasyAlgorithmData : BotAlgorithmData
    {
        
        private Dictionary<PieceDragHandler, List<Vector2Int>> m_reusablePossibleMovesDic =
            new Dictionary<PieceDragHandler, List<Vector2Int>>();

        private List<PieceMove> m_reusableFlattenPossibleMovesByPiece =
            new List<PieceMove>();
        
        public override IEnumerator CalculateBestMoveCo(BoardController boardController, PlayerInteractionHandler botPlayer, Action<PieceMove> onFinished)
        {
            bool allPiecesOnBoard = botPlayer.AllPiecesOnBoard();
            foreach (var pieceDragHandler in botPlayer.Pieces)
            {
                if (!m_reusablePossibleMovesDic.TryGetValue(pieceDragHandler,
                        out List<Vector2Int> possibleMovesByPiece))
                {
                    possibleMovesByPiece = m_reusablePossibleMovesDic[pieceDragHandler] = new List<Vector2Int>();
                }
                possibleMovesByPiece.Clear();
                if (allPiecesOnBoard)
                {
                    pieceDragHandler.Data.PopulateWithValidMoves(boardController, pieceDragHandler.GridPosition, possibleMovesByPiece);
                }
                else if(!pieceDragHandler.IsPlacedOnBoard) // take only pieces not on board
                {
                    AIBoardGridHelper.PopulateWithAllFreePositions(boardController, possibleMovesByPiece);
                }
                yield return null;
            }

            m_reusableFlattenPossibleMovesByPiece.Clear();
            m_reusableFlattenPossibleMovesByPiece.AddRange(m_reusablePossibleMovesDic
                .SelectMany(kvp =>
                    kvp.Value.Select(move => new PieceMove(kvp.Key, move))));


            // Check if no movement is possible and in that case ends with a tie.
            if (m_reusableFlattenPossibleMovesByPiece.Count == 0)
            {
                onFinished?.Invoke(default);
                yield break;
            }
            
            // Just get a random valid movement
            int randIdx = Random.Range(0, m_reusableFlattenPossibleMovesByPiece.Count);
            var pieceMove = m_reusableFlattenPossibleMovesByPiece[randIdx];
            onFinished?.Invoke(pieceMove);
            yield return null;
        }
    }
}