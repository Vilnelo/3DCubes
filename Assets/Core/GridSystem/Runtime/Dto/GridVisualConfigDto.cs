using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.GridSystem.Runtime.Dto
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class GridVisualConfigDto
    {
        [SerializeField, JsonProperty("gridSize")]
        private int m_GridSize;

        [SerializeField, JsonProperty("cubeScale")]
        private float m_CubeScale;

        [SerializeField, JsonProperty("spacing")]
        private float m_Spacing;

        [SerializeField, JsonProperty("colorMappings")]
        private Dictionary<string, string> m_ColorMappings;

        public int GridSize => m_GridSize;
        public float CubeScale => m_CubeScale;
        public float Spacing => m_Spacing;
        public IReadOnlyDictionary<string, string> ColorMappings => m_ColorMappings;
    }
}