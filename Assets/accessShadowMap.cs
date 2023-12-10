using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// enables access of shadow map of this light in the shader files
// posted by flogelz at https://forum.unity.com/threads/shader-access-to-shadow-map.58240/
namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class CopyShadowMap : MonoBehaviour
    {
        CommandBuffer cb = null;

        void OnEnable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                cb = new CommandBuffer();
                cb.name = "CopyShadowMap";
                cb.SetGlobalTexture("_DirectionalShadowMask", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                light.AddCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }

        void OnDisable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                light.RemoveCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }
    }
}