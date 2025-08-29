using System.Collections.Generic;
using UnityEngine;

namespace Core.GridSystem.External
{
    public class MaterialCache
    {
        private readonly Dictionary<int, Material> m_CachedMaterials = new Dictionary<int, Material>();

        public Material GetMaterial(int colorCode, Color color)
        {
            if (!m_CachedMaterials.TryGetValue(colorCode, out var material))
            {
                material = CreateMaterial(color);
                m_CachedMaterials[colorCode] = material;
                
                Debug.Log($"[MaterialCache] Created material for color code: {colorCode}, color: {color}");
            }

            return material;
        }

        private Material CreateMaterial(Color color)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        public void Clear()
        {
            foreach (var material in m_CachedMaterials.Values)
            {
                if (material != null)
                {
                    Object.DestroyImmediate(material);
                }
            }
            
            m_CachedMaterials.Clear();
            Debug.Log("[MaterialCache] Cleared all materials");
        }
    }
}