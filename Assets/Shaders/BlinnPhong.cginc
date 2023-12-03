#include "UnityCG.cginc"

struct v2f
{
    float4 clipPos : SV_POSITION;
    // Clip position, which transforms vertices coordinate to pixel coordinate on screen. (You won't need this)
    float3 worldPos : TEXTCOORD0; // World position of vertex
    float3 worldNormal : NORMAL; // World normal of vertex
};

v2f MyVertexProgram(appdata_base v)
{
    v2f o;
    // World position
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Clip position
    o.clipPos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1.));

    // Normal in WorldSpace
    o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));


    return o;
}

half4 _LightColor0;

// Diffuse
fixed4 _DiffuseColor;

//Specular
fixed _Shininess;
fixed4 _SpecularColor;


// Emission
fixed4 _EmissionColor;


fixed4 MyFragmentProgram(v2f i) : SV_Target
{
    //TODO: Calculate the Blinn-Phong specular model here. The diffuse component is already calculated for you,
    // but the "attenuation" quantity is not determined yet. You will need to:
    // (1) Calculate the distance attenuation for point light using the formula 1/r^2 where r is the distance
    // between a point light and a surface vertex.
    // (2) Calculate the specular component. You will need to define more vectors similar to the ones in class
    // (3) Obtain the ambient component using Unity's built-in variable.
    // 
    // Hint: Check out  https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
    // to learn more about built-in shader variables. You will need _WorldSpaceLightPos0,
    // _WorldSpaceCameraPos, UNITY_LIGHTMODEL_AMBIENT, and _LightColor0.
    // 
    // Hint: argument "i" is a vertex data of type "v2f" defined above. Use this data to calculate appropriate
    // directional vectors (camera view, normal, light)

    float4 color = float4(0, 0, 0, 1);
    color.rgb += _EmissionColor.rgb;

    float3 L = float3(0, 0, 0); // Light direction

    // caculate attenuation
    float r = distance(i.worldPos, _WorldSpaceLightPos0);
    float attenuation = 1.0f / (1.0f + pow(r, 2.0f));

    if (_WorldSpaceLightPos0.w != 0.0) // this is point light
    {
        L = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
    }
    float3 N = normalize(i.worldNormal);

    // Diffuse component
    float3 diffuse = float3(0, 0, 0);
    float diffuseShade = max(dot(N, L), 0.0);
    diffuse = diffuseShade * _DiffuseColor * _LightColor0 * attenuation;

    // specular component 
    float3 specular = float3(0, 0, 0);
    float3 V = float3(0, 0, 0);
    float3 H = float3(0, 0, 0);

    // get halfway vector
    V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
    H = normalize(L + V);
    float3 specularShade = max(0.0, dot(N, H));

    // apply exponent 
    specularShade = pow(specularShade, _Shininess);
    specular = specularShade * _SpecularColor * _LightColor0 * attenuation;
    
    // ambient component
    //float3 ambient = float3(0, 0, 0);
    float3 ambient = UNITY_LIGHTMODEL_AMBIENT;

    color.rgb += ambient + diffuse + specular;

    return color;
}
