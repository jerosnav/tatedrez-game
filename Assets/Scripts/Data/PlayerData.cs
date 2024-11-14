using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Player Data", fileName = "New Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private  Color m_playerColor = Color.white;

    public Color PlayerColor => m_playerColor;
}
