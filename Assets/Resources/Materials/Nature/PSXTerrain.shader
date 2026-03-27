 Shader "Custom/TerrainPSXVertexJitter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _JitterStrength ("Jitter Strength", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _JitterStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Create consistent "jitter" using world position
                float3 pos = v.vertex.xyz;
                float2 jitterCoord = pos.xz * 10; // scale affects "grid"
                float offset = (hash(jitterCoord) - 0.5) * 2.0 * _JitterStrength;

                // Apply jitter on y-axis only (simulate Z-fighting or low precision)
                pos.y += offset;

                o.vertex = UnityObjectToClipPos(float4(pos, 1.0));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}

