Shader "Custom/InnerWorldReveal"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)

        _RevealCenter ("Reveal Center (world)", Vector) = (0, 1, 0, 0)
        _RevealRadius ("Reveal Radius", Float) = 3

        // 里世界整体色调
        _InnerTint ("Inner World Tint", Color) = (0.6, 0.8, 1.0, 1)

        // 边缘宽度 & 发光颜色
        _EdgeWidth ("Edge Width", Float) = 1
        _EdgeColor ("Edge Glow Color", Color) = (0.3, 0.9, 1.0, 1)
        _EdgeIntensity ("Edge Glow Intensity", Float) = 2
    }

    SubShader
    {
        // 仍然是非透明，但稍微后画一点
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        float4 _RevealCenter;
        float  _RevealRadius;

        fixed4 _InnerTint;
        float  _EdgeWidth;
        fixed4 _EdgeColor;
        float  _EdgeIntensity;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 距离遮罩中心的距离
            float dist = distance(IN.worldPos, _RevealCenter.xyz);

            // 超出可见半径的像素直接丢弃
            if (dist > _RevealRadius)
            {
                clip(-1);
            }

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // 1. 里世界整体色调偏移
            fixed3 baseColor = c.rgb * _InnerTint.rgb;

            // 2. 计算一个“边缘 ring”用来发光
            float edgeWidth = max(_EdgeWidth, 0.0001);

            // t: 0 在边界处 (dist ~= _RevealRadius)，1 在内侧 (dist <= _RevealRadius - edgeWidth)
            float t = saturate((_RevealRadius - dist) / edgeWidth);

            // ring: 在边缘中间最亮，内外两侧逐渐变暗（0~1）
            float ring = t * (1.0 - t) * 4.0;

            o.Albedo   = baseColor;
            o.Alpha    = c.a;
            o.Emission = _EdgeColor.rgb * ring * _EdgeIntensity;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
