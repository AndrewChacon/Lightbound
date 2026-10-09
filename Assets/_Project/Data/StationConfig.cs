using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "StationConfig", menuName = "Lightbound/Station Config")]
    public class StationConfig : ScriptableObject
    {
        [Header("Lantern Battery station")]
        public float chargeTime = 3f;
        public float zoneRadius = 5f;
        [Range(0f, 1f)] public float restorePercent = 1f;
    }
}
