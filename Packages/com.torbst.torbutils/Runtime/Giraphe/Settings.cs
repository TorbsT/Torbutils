using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TorbuTils.Giraphe
{
    public class Settings : MonoBehaviour
    {
        public static string PositionSatellite
        {
            get
            {
                if (Instance == null) return defaultPositionSatellite;
                return Instance.CustomPositionSatellite;
            }
        }
        public static string ColorSatellite
        {
            get
            {
                if (Instance == null) return defaultColorSatellite;
                return Instance.CustomColorSatellite;
            }
        }
        public static string CostSatellite
        {
            get
            {
                if (Instance == null) return defaultCostSatellite;
                return Instance.CustomCostSatellite;
            }
        }

        internal static Settings Instance { get; private set; }
        [field: SerializeField]
        internal string CustomPositionSatellite
        { get; private set; } = defaultPositionSatellite;
        [field: SerializeField]
        internal string CustomColorSatellite
        { get; private set; } = defaultColorSatellite;
        [field: SerializeField]
        internal string CustomCostSatellite
        { get; private set; } = defaultCostSatellite;

        private readonly static string defaultPositionSatellite = "pos"; 
        private readonly static string defaultColorSatellite = "color"; 
        private readonly static string defaultCostSatellite = "cost"; 

        private void Awake()
        {
            Instance = this;
        }
    }
}
