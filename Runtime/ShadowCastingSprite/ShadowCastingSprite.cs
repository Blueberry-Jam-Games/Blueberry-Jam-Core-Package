using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BJ
{
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteAlways]
    public class ShadowCastingSprite : MonoBehaviour
    {

        [SerializeField]
        private Material litMaterial;

        private void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            spriteRenderer.receiveShadows = true;
        }

#if UNITY_EDITOR
        private SpriteRenderer targetRenderer;

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if(targetRenderer == null)
                {
                    targetRenderer = GetComponent<SpriteRenderer>();
                    bool needSave = false;
                    if(targetRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.TwoSided)
                    {
                        targetRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                        needSave = true;
                    }
                    if(targetRenderer.receiveShadows != true)
                    {
                        targetRenderer.receiveShadows = true;
                        needSave = true;
                    }
                    if (targetRenderer.sharedMaterial != litMaterial)
                    {
                        targetRenderer.sharedMaterial = litMaterial;
                        needSave = true;
                    }
                    if(needSave)
                    {
                        EditorUtility.SetDirty(this.gameObject);
                    }
                }
            }
        }
#endif
    }
}
