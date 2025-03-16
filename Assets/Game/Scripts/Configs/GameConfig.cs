using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Config
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptlable Objects/Settings/GameSettings")]
    public class GameConfig : ScriptableObject
    {
        [Header("Stack Settings")] 
        public float stackLength;
        public Vector3 defaultStackScale;
        public float slowestPingPongSpeed;
        public float perfectDist;
        public Material[] stackMaterials;
        public Material finishLineMaterial;
        public LayerMask groundLayer;

        public Material GetRandomMaterial()
        {
            return stackMaterials[Random.Range(0, stackMaterials.Length)];
        }
    }
}