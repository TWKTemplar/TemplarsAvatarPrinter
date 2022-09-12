Shader "TemplarsShaders/3DPrinterParticle1"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ColorIntensity("Intensity", Range(0,10)) = 4
        _Cutoff("Cutoff", Range(0,1)) = 0.5
        _ColorLerp("Color Lerp", Range(0,1)) = 0.5
        _ParticleSize("ParticleSize", Range(0,1)) = 0.05//Number that will be subtracted from xyz of TEXCOORD1(Particle size)

    }
        SubShader
    {
        //Tags { "RenderType" = "Opaque" }

       Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest" "IgnoreProjector" = "True" "PreviewType" = "Plane" "DisableBatching" = "True" }
       //Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Plane"}
        LOD 100
        Blend One OneMinusSrcAlpha// Traditional transparency // Traditional transparency
        //Blend One One // Additive blending.
        ZWrite Off // Depth test off.

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                float3 VertexColor : TEXCOORD1;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float3 VertexColor : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ColorLerp;
            float _Cutoff;
            float _ColorIntensity;
            float _ParticleSize;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                //ty angriest
                float3 VertexColor2 = v.VertexColor - _ParticleSize.xxx;
                VertexColor2 = VertexColor2 * 1000;
                o.VertexColor = VertexColor2;

                o.uv.z = v.uv.z/ UNITY_TWO_PI;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float4 hue = tex2D(_MainTex, i.uv);
                clip(hue.a - _Cutoff);
                hue.rgb *= hsv2rgb(i.VertexColor);// * pow(col.a,5)
                hue.rgb *= _ColorIntensity ;
                hue.rgb = lerp(hue.rgba,tex2D(_MainTex, i.uv).rgba, _ColorLerp);
                return hue;
            }
            ENDCG
        }
    }
}