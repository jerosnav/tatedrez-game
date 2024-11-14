using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    public class TeamData : ScriptableObject
    {
        [SerializeField] private Color m_color = Color.white;

        public Color Color => m_color;
    }
}