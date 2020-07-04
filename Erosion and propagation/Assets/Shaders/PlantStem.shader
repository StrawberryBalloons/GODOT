// NOTE: only works properly with deferred rendering path.
// With forward rendering, shading will remain floating in air, and will block shadows on the ground (unless queue is
// set to transparent, but then won't receive shadows/write to depth texture)
// https://forum.unity.com/threads/custom-lighting-function-breaks-dithering.707096/

//https://ocias.com/blog/unity-stipple-transparency-shader/
//https://forum.unity.com/threads/clip-in-surface-shader-doesnt-clip-ligthing.470284/
//https://realtimevfx.com/t/transparency-issues-in-unity/4337/2
Shader "Custom/Plant Stem" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
    _FadeDistance ("Fade Distance", Float) = 1.0
    _GrowthPercent ("Growth Percent", Range(0, 1)) = 1
}
SubShader {

    Cull Off
    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Geometry" }
    LOD 200
    CGPROGRAM
    #pragma surface surf Standard addshadow
    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0
    sampler2D _MainTex;
    

    struct Input {
        float2 uv_MainTex;
        float4 screenPos;
        float3 worldPos;
    };
    float _FadeDistance;
    fixed4 _Color;
    float _GrowthPercent;


    void surf (Input IN, inout SurfaceOutputStandard o) {
        float l = length(mul(unity_CameraInvProjection, float4(1,1,0,1)).xyz) * _ProjectionParams.y;
        float3 cameraPos = _WorldSpaceCameraPos;
        float dst = length(IN.worldPos - cameraPos);
        float fade = saturate((dst-l) / _FadeDistance);


        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = _Color;
        o.Metallic = 0;
        o.Smoothness = 0;
        o.Alpha = c.a;
        
        float amountHidden = 1-saturate(_GrowthPercent);
        clip(1-IN.uv_MainTex.y - amountHidden);
     
        
        // Screen-door transparency: Discard pixel if below threshold.
        float4x4 thresholdMatrix =
        {  1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
        };
        float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
        float2 pos = IN.screenPos.xy / IN.screenPos.w;
        pos *= _ScreenParams.xy; // pixel position
        clip(fade - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
        
    }
ENDCG
}
FallBack "VertexLit"
}