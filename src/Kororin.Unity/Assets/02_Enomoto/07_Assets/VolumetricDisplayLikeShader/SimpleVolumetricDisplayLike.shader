Shader "kaiware007/Simple Volumetric Display Like"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Color("Color", Color) = (0.3162602, 0.5294864, 0.9716981, 0.5019608)
        _RimPower("RimPower", Range(0,10)) = 5.0
        
        [Space]
        _DistortionNoisePower("Distortion Noise Power", float) = 0.0005
        _LargeDistortionNoisePower("Large Distortion Noise Power", float) = 0.025
        
        [Space]
        _VStripeFreq("Vertical Stripe Frequency", float) = 500
        _VStripeSpeed("Vertical Stripe Speed", float) = 333
        _VStripeIntensity("Vertical Stripe Intensity", float) = 1.5

        [Space]
        _HStripeFreq("Horizontal Stripe Frequency", float) = 1
        _HStripeSpeed("Horizontal Stripe Speed", float) = 500
        _HStripeIntensity("Horizontal Stripe Intensity", float) = 3
        _HOffset("Horizontal Stripe Offset XZ", vector) = (0,0,0,0)
        
        [Toggle(DOUBLESIDE)]
        _DoubleSide("Double Side", float) = 0
    }

    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        #include "Noise.cginc"
        
        #define INPUT_PATCH_SIZE 3
		#define OUTPUT_PATCH_SIZE 3
        
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
            uint vid : SV_VertexID;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
            float3 normalDir : TEXCOORD2;
            float3 worldPos : TEXCOORD3;
            float3 localPos : TEXCOORD4;
            float3 worldCenter : TEXCOORD5;
            UNITY_FOG_COORDS(6)
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };
        
        sampler2D _MainTex;
        float4 _MainTex_ST;

        float4 _Color;
        half _RimPower;
        
        float _VStripeFreq;
        float _VStripeSpeed;
        float _VStripeIntensity;

        float _HStripeFreq;
        float _HStripeSpeed;
        float _HStripeIntensity;
        float2 _HOffset;
        
        float _DistortionNoisePower;
        float _LargeDistortionNoisePower;
        
        float mod(float x, float y) {
            return x - y * floor(x / y);
        }

        v2f vert(appdata v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            float4 pos = v.vertex;

            float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
            float4 wpos = mul(unity_ObjectToWorld, pos);
            float3 lpos = wpos.xyz - worldCenter;

            // Glitch
            float t = fmod(_Time.y * 2000, UNITY_PI * 2);
            float t2 = fmod(_Time.y * 30.1, UNITY_PI * 2);
            float per = pow(snoise(float2(793.4235, _Time.y * 1.0))* 0.5 + 0.5, 2.5) >= 0.025 ? 0 : 1;
            wpos.xz += sin(lpos.y * 2000 + t) * _DistortionNoisePower + clamp(cos(lpos.y * 15.1 + t2) * per * 5, -1, 1) * _LargeDistortionNoisePower;
            pos = mul(unity_WorldToObject, wpos);
            
            o.position = UnityObjectToClipPos(pos);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);

            // rim light
            o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
            o.viewDir = normalize(_WorldSpaceCameraPos - wpos.xyz);
            o.worldPos = wpos;
            o.localPos = lpos;
            o.worldCenter = worldCenter;
            UNITY_TRANSFER_FOG(o, o.position);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv) * _Color;

            // rim light
            half rim = 1.0 - abs(dot(i.viewDir, i.normalDir));
            fixed3 emission = _Color.rgb * pow(rim, _RimPower) * _RimPower;
            col.rgb += emission;

            // 距離で縦シマの周波数や速度を変える
            // Vertical stripes change frequency and speed with distance from the camera.
            float camDist = 1.0 / length(i.worldCenter.xz - _WorldSpaceCameraPos.xz);
            camDist = camDist > 1.0 ? 1.0 / camDist : 1;
            float vfreq =  _VStripeFreq * camDist;
            float vspd = _VStripeSpeed * camDist;
            
            // scan line(vertical)
            float vscan = smoothstep(0.5, 1.0, sin(i.worldPos.y * vfreq + _Time.y * vspd));
            col.rgb += vscan * _Color.rgb * _VStripeIntensity;
            
            // large scan line(vertical)
            col.rgb += smoothstep(-0.5, 1.0, sin(i.worldPos.y * vfreq * 0.025 - _Time.y * 233.335)) * _Color.rgb * 0.25;

            // scan line(horizontal)
            float2 hpos = _HOffset + i.localPos.xz;
            col.rgb += smoothstep(0.5 * UNITY_PI, UNITY_PI, mod(atan2(hpos.y, hpos.x) * _HStripeFreq * camDist - _Time.y * _HStripeSpeed * camDist, UNITY_PI)) * _Color.rgb * _HStripeIntensity * vscan;
            
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col;
        }

        fixed4 fragWithBackMask(v2f i) : SV_Target
        {
#ifdef DOUBLESIDE
            return frag(i);
#else
            return 0;
#endif
        }
        ENDCG

        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Cull Front
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragWithBackMask
            #pragma shader_feature DOUBLESIDE
            #pragma multi_compile_instancing
            ENDCG
        }
        
        Cull Back
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            ENDCG
        }
    }
}
