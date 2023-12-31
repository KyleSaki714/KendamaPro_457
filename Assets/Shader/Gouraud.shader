Shader "Custom/Lighting/Gouraud"
{
    Properties
    {
	    [Header(Diffuse)]
        _DiffuseColor ("Color", color) = (1., 1., 1., 1.)
 
    	[Header(Emission)]
        _EmissionColor ("Color", color) = (0., 0., 0., 0.)
        _Shininess ("Shininess", Float) = 1.
    	
        [Header(Specular)]
        _SpecularColor ("Specular color", color) = (0., 0., 0., 1.)
        
        [Header(Transparency)]
        _TransparencyColor ("Color", color) = (0., 0., 0., 0.)
        _TransparencyIndex("Index Of Refraction", float) = 0.

		_MainTex ("Texture", 2D) = "white" {}
    }
 
    SubShader {

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile _ VERTEXLIGHT_ON

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			// make fog work
            #pragma multi_compile_fog

			#include "Gouraud.cginc"

			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}

			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_fwdadd

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			// make fog work
            #pragma multi_compile_fog

			#include "Gouraud.cginc"

			ENDCG
		}
    }
}