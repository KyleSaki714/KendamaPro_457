#include "UnityCG.cginc"

struct v2f
{
    float4 clipPos : SV_POSITION;
    // Clip position, which transforms vertices coordinate to pixel coordinate on screen. (You won't need this)
    float3 worldPos : TEXTCOORD0; // World position of vertex
    float3 worldNormal : NORMAL; // World normal of vertex
    float4 color : COLOR4; // vertex determined color
    float2 uv : TEXCOORD0; // texture coordinate
};

half4 _LightColor0;

// Diffuse
fixed4 _DiffuseColor;

//Specular
fixed _Shininess;
fixed4 _SpecularColor;

// Emission
fixed4 _EmissionColor;


v2f MyVertexProgram(appdata_base v) 
{
    v2f o;
    // World position
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Clip position
    o.clipPos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.));

    // Normal in WorldSpace
    o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

    // SET UP COLOR
    float4 color = float4(0, 0, 0, 1);
    color.rgb += _EmissionColor.rgb;

    float3 L = float3(0, 0, 0); // Light direction

    // caculate attenuation
    float r = distance(o.worldPos, _WorldSpaceLightPos0);
    float attenuation = 1.0f / (1.0f + pow(r, 2.0f));

    if (_WorldSpaceLightPos0.w != 0.0) // this is point light
    {
        L = normalize(_WorldSpaceLightPos0.xyz - o.worldPos);
    }
    float3 N = normalize(o.worldNormal);

    // ambient
    float3 ambient = UNITY_LIGHTMODEL_AMBIENT;

    // DIFFUSE COMPONENT
    float3 diffuse = float3(0, 0, 0);
    float diffuseShade = max(dot(N, L), 0.0);
    diffuse = diffuseShade * _DiffuseColor * _LightColor0 * attenuation;

    // SPECULAR COMPONENT
    float3 specular = float3(0, 0, 0);
    float3 V = float3(0, 0, 0);
    float3 H = float3(0, 0, 0);

    // get halfway vector
    V = normalize(_WorldSpaceCameraPos.xyz - o.worldPos);
    H = normalize(L + V);
    float3 specularShade = max(0.0, dot(N, H));

    // apply exponent 
    specularShade = pow(specularShade, _Shininess);
    specular = specularShade * _SpecularColor * _LightColor0 * attenuation;

    
    color.rgb += ambient + diffuse + specular;

    o.color = color;

    // handle texture
    o.uv = v.texcoord;

    return o;
}

 // texture we will sample
sampler2D _MainTex;

fixed4 MyFragmentProgram(v2f i) : SV_Target
{
    // sample texture and return it
    fixed4 textureColor = tex2D(_MainTex, i.uv);

    return textureColor * i.color;
}
