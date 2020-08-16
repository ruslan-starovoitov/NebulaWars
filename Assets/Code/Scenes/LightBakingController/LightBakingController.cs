using System;
using UnityEngine;

namespace Code.Scenes.LightBakingController
{
    public class LightBakingController : MonoBehaviour
    {
        private void Awake()
        {
            Bake();
        }

        private void Bake()
        {
            //3d модель
            Mesh meshToModify = GetComponent<MeshCollider>().sharedMesh;
            //текстура освещения
            Vector4 lightmapOffsetAndScale = GetComponent<Renderer>().lightmapScaleOffset;

            //Новая 3d модель с вшитым светом
            Vector2[] modifiedUv2S = meshToModify.uv2;
            for (int i = 0; i < meshToModify.uv2.Length; i++)
            {
                float x = meshToModify.uv2[i].x * lightmapOffsetAndScale.x + lightmapOffsetAndScale.z;
                float y = meshToModify.uv2[i].y * lightmapOffsetAndScale.y + lightmapOffsetAndScale.w;
                modifiedUv2S[i] = new Vector2(x, y);
            }
            meshToModify.uv2 = modifiedUv2S;
        }
    }
}