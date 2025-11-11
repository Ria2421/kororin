Shader "kaiware007/Volumetric Display Like"
{
    Properties
    {
        _Tessellation ("Tessellation Amount", Range(0.01, 10)) = 1
        
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaMask("Alpha Mask", 2D) = "white" {}
        _CutOff("Cut Off", Range(0,1)) = 0.5

        _Color("Color", Color) = (0.3162602, 0.5294864, 0.9716981, 0.5019608)
        _RimPower("RimPower", Range(0,10)) = 5.0
        
        [Space]
        _DistortionNoisePower("Distortion Noise Power", float) = 0.0005
        _LargeDistortionNoisePower("Large Distortion Noise Power", float) = 0.025
        
        [Space]
        _TopScatterHeight("Top Scatter Height", float) = 1
        _BottomScatterHeight("Bottom Scatter Height", float) = -1
        _ScatterPer("Scatter Per", Range(0,1)) = 0.8

        [Space]
        _VStripeFreq("Vertical Stripe Frequency", float) = 500
        _VStripeSpeed("Vertical Stripe Speed", float) = 333
        _VStripeIntensity("Vertical Stripe Intensity", float) = 1.5

        [Space]
        _HStripeFreq("Horizontal Stripe Frequency", float) = 1
        _HStripeSpeed("Horizontal Stripe Speed", float) = 500
        _HStripeIntensity("Horizontal Stripe Intensity", float) = 3
        _HOffset("Horizontal Stripe Offset XZ", vector) = (0,0,0,0)

        [Space]
        _DecimePer("Decimation Probability", Range(0,1)) = 0.5
        
        _ScatterMask ("Scatter Mask", 2D) = "white" {}
        
        [Space]
        _ScatterLife("Scatter Triangle Life Time", float) = 0.2
        _ScatterSpeed("Scatter Triangle Speed", float) = 0.05

        [Space]
        _EdgeThickness("Scatter Edge Thickness", float) = 0.01
        _Gain("Scatter Edge Gain", float) = 1.5
        
        [Toggle(DOUBLESIDE)]
        _DoubleSide("Double Side", float) = 1
    }

    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        #include "Tessellation.cginc"
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

        struct v2h
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
			float vid : TEXCOORD1;
            float3 normal : NORMAL;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
        };

		struct h2d_main
        {
            float4 vertex : SV_POSITION;
		    float2 uv : TEXCOORD0;
			float vid : TEXCOORD1;
		    float3 normal : NORMAL;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
        };

		struct h2d_const {
			float tess_factor[3] : SV_TessFactor;
            float InsideTessFactor : SV_InsideTessFactor;
        };
        
        struct d2g
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
            uint vid : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct g2f
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
            float3 normalDir : TEXCOORD2;
            float3 worldPos : TEXCOORD3;
            float3 localPos : TEXCOORD4;
            float4 bary : TEXCOORD5;
            float3 worldCenter : TEXCOORD6;
            UNITY_FOG_COORDS(7)
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        float _Tessellation;
        
        sampler2D _MainTex;
        float4 _MainTex_ST;
        sampler2D _AlphaMask;
        float _CutOff;

        float4 _Color;
        half _RimPower;

        float _TopScatterHeight;
        float _BottomScatterHeight;
        float _ScatterPer;
        
        float _VStripeFreq;
        float _VStripeSpeed;
        float _VStripeIntensity;

        float _HStripeFreq;
        float _HStripeSpeed;
        float _HStripeIntensity;
        float2 _HOffset;

        float _DecimePer;

        sampler2D _ScatterMask;
        
        float _ScatterLife;
        float _ScatterSpeed;

        float _EdgeThickness;
        float _Gain;

        float _DistortionNoisePower;
        float _LargeDistortionNoisePower;
        
        // Quaternion multiplication.
        // http://mathworld.wolfram.com/Quaternion.html
        float4 qmul(float4 q1, float4 q2)
        {
            return float4(
                q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
                q1.w * q2.w - dot(q1.xyz, q2.xyz)
                );
        }

        // Rotate a vector with a rotation quaternion.
        // http://mathworld.wolfram.com/Quaternion.html
        float3 rotateWithQuaternion(float3 v, float4 r)
        {
            float4 r_c = r * float4(-1, -1, -1, 1);
            return qmul(r, qmul(float4(v, 0), r_c)).xyz;
        }

        float4 getAngleAxisRotation(float3 axis, float angle) {
            axis = normalize(axis);
            float s, c;
            sincos(angle, s, c);
            return float4(axis.x * s, axis.y * s, axis.z * s, c);
        }

        float3 rotateAngleAxis(float3 v, float3 axis, float angle) {
            float4 q = getAngleAxisRotation(axis, angle);
            return rotateWithQuaternion(v, q);
        }

        float4 fromToRotation(float3 from, float3 to) {
            float3
                v1 = normalize(from),
                v2 = normalize(to),
                cr = cross(v1, v2);
            float4 q = float4(cr, 1 + dot(v1, v2));
            return normalize(q);
        }

        float4 eulerToQuaternion(float3 axis, float angle) {
            return float4 (
                axis.x * sin(angle * 0.5),
                axis.y * sin(angle * 0.5),
                axis.z * sin(angle * 0.5),
                cos(angle * 0.5)
                );
        }

        float rand(float2 co) {
            return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
        }

        float3 rand3(float2 seed) {
            float t = sin(seed.x + seed.y * 1e3);
            return float3(frac(t * 1e4), frac(t * 1e6), frac(t * 1e5));
        }

        float3 randInsideUnitSphere(float2 seed) {
            return (rand3(seed) - 0.5) * 2.0;
        }

        //  1 out, 2 in...
        float hash12(float2 p)
        {
	        float3 p3  = frac(float3(p.xyx) * .1031);
            p3 += dot(p3, p3.yzx + 33.33);
            return frac((p3.x + p3.y) * p3.z);
        }

        float mod(float x, float y) {
            return x - y * floor(x / y);
        }

        float edgeFactor(float3 bary)
        {
            float3 d = fwidth(bary);
            float3 a3 = smoothstep(float3(0, 0, 0), _Gain * d, bary);
            return min(min(a3.x, a3.y), a3.z);
        }

        v2h vert(appdata v)
        {
            v2h o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2h, o);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            float4 pos = v.vertex;

            float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1));
            float4 wpos = mul(unity_ObjectToWorld, pos);
            float3 lpos = wpos - worldCenter;

            // Glitch
            float t = fmod(_Time.y * 2000, UNITY_PI * 2);
            float t2 = fmod(_Time.y * 30.1, UNITY_PI * 2);
            float per = pow(snoise(float2(793.4235, _Time.y * 1.0))* 0.5 + 0.5, 2.5) >= 0.025 ? 0 : 1;
            wpos.xz += sin(lpos.y * 2000 + t) * _DistortionNoisePower + clamp(cos(lpos.y * 15.1 + t2) * per * 5, -1, 1) * _LargeDistortionNoisePower;
            pos = mul(unity_WorldToObject, wpos);
            
            o.vertex = pos;
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.normal = v.normal;
            o.vid = v.vid;
            return o;
        }

        h2d_const HullConst(InputPatch<v2h, INPUT_PATCH_SIZE> i) {
            h2d_const o = (h2d_const)0;
            
			float3 retf;
            float  ritf, uitf;
            ProcessTriTessFactorsAvg(_Tessellation.xxx, 1, retf, ritf, uitf );
            
             o.tess_factor[0] = retf.x;
             o.tess_factor[1] = retf.y;
             o.tess_factor[2] = retf.z;
             o.InsideTessFactor = ritf;
            return o;
        }

		[domain("tri")]
        [partitioning("integer")]
        [outputtopology("triangle_cw")]
        [outputcontrolpoints(OUTPUT_PATCH_SIZE)]
        [patchconstantfunc("HullConst")]
        h2d_main Hull(InputPatch<v2h, INPUT_PATCH_SIZE> i, uint id:SV_OutputControlPointID) {
            h2d_main o = (h2d_main)0;
			UNITY_SETUP_INSTANCE_ID(i[0]);
			UNITY_TRANSFER_INSTANCE_ID(i[0], o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			o.vertex = i[id].vertex;
            // o.vid = i[id].vid;
            o.vid = rand(float2(i[id].vid, (float)id * 676.13558));
		    o.uv = i[id].uv;
            o.normal = i[id].normal;
            return o;
        }

		[domain("tri")]
        d2g Domain(h2d_const hs_const_data,  OutputPatch<h2d_main, 3> i, float3 bary: SV_DomainLocation)
        {
            d2g o;
			UNITY_SETUP_INSTANCE_ID(i[0]);
			UNITY_TRANSFER_INSTANCE_ID(i[0], o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.vertex = i[0].vertex * bary.x + i[1].vertex * bary.y + i[2].vertex * bary.z;
			o.vid = rand(float2(i[0].vid * o.vertex.x, i[1].vid * o.vertex.y + i[2].vid * o.vertex.z));
            o.uv = i[0].uv * bary.x + i[1].uv * bary.y + i[2].uv * bary.z;
            o.normal = normalize(i[0].normal * bary.x + i[1].normal * bary.y + i[2].normal * bary.z);
            return o;
        }
        
        [maxvertexcount(3)]
        void geom(triangle d2g input[3], uint pid : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
        {
            // 中心点計算
            // Center point calculation
            float3 center = (input[0].vertex.xyz + input[1].vertex.xyz + input[2].vertex.xyz) / 3;
            float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1));
            
            float2 centerUV = (input[0].uv.xy + input[1].uv.xy + input[2].uv.xy) / 3;
            float scatterMask = 1-tex2Dlod(_ScatterMask, float4(centerUV,0,0)).r;
            float scatterMaskScale = scatterMask * (sin(hash12(float2(pid * 5.16 + input[2].vid, input[0].vid + 1634.161)) * UNITY_PI + _Time.y * 2.5))*0.2;

            float3 wpos = mul(unity_ObjectToWorld, float4(center,1));
            float3 lpos = wpos - worldCenter;
            
            // 散り判定
            // Scatter judgement
            float scatter = max(0, lpos.y - _TopScatterHeight) + min(0, lpos.y - _BottomScatterHeight);
            float scatterEdge = max(0, lpos.y - (_TopScatterHeight - _EdgeThickness)) + min(0, lpos.y - (_BottomScatterHeight + _EdgeThickness)) + scatterMask;

            // 散り移動量計算
            // Scatter movement calculation
            float isScatter = step(1 - _ScatterPer, rand(float2(pid, 190.289 + input[0].vid)) + abs(scatter) * 10);
            float3 axis = randInsideUnitSphere(float2(5613.5661 + input[2].vid, pid * 41.164));
            float l = mod(_Time.y * _ScatterSpeed + rand(float2(pid * 5.516, pid + input[1].vid) * 13.456), _ScatterLife);
            float yy = isScatter * l * sign(scatter);
            
            // 散るものでポリゴンを間引く確率判定
            // Probability determination to thin out polygons by scattering
            float per = hash12(float2(pid - 193.11279, pid * 190.163)) * 0.5 + 0.5 - abs(scatter);
            if((per < _DecimePer) && (abs(scatterEdge) > 0.) || (scatterMask >= 1.))
                return;

            
            scatter *= isScatter;
                
            [unloop]
            for (int i = 0; i < 3; i++)
            {
                g2f o;
                UNITY_INITIALIZE_OUTPUT(g2f, o);
                UNITY_SETUP_INSTANCE_ID(input[i]);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[i]);
                UNITY_TRANSFER_INSTANCE_ID(input[i], o);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[i], o);

                float4 lpos = input[i].vertex;
                
                // scatter rotate
                lpos.xyz += (center - lpos.xyz ) * (scatterMask + scatterMaskScale);
                lpos.xyz += (center - lpos.xyz ) * (l / _ScatterLife) * (abs(scatter) > 0);
                lpos.xyz = abs(scatter) > 0 ? center + rotateAngleAxis(lpos.xyz - center, axis, _Time.y * 0.8) : lpos.xyz;

                float3 wpos = mul(unity_ObjectToWorld, lpos);
                wpos.y += yy;
                
                float4 pos = mul(UNITY_MATRIX_VP, float4(wpos,1));
                o.position = pos;
                o.uv = input[i].uv;

                // rim light
                o.normalDir = normalize(UnityObjectToWorldNormal(input[i].normal));
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, input[i].vertex).xyz);
                o.worldPos = wpos;
                o.localPos = lpos.xyz;
                
                // wireframe
                o.bary = float4(i == 0, i == 1, i == 2, scatterEdge);

                o.worldCenter = worldCenter;
                UNITY_TRANSFER_FOG(o, o.position);
                outStream.Append(o);
            }
            outStream.RestartStrip();
        }

        fixed4 frag(g2f i) : SV_Target
        {
            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv) * _Color;
            fixed mask = tex2D(_AlphaMask, i.uv).r;

            clip(mask - _CutOff);

            // rim light
            half rim = 1.0 - abs(dot(i.viewDir, i.normalDir));
            fixed3 emission = _Color.rgb * pow(rim, _RimPower) * _RimPower;
            col.rgb += emission;

            // 距離で縦シマの周波数や速度を変える
            // Vertical stripes change frequency and speed with distance from the camera.
            float camDist = length(i.worldCenter.xz - _WorldSpaceCameraPos.xz);
            camDist = camDist > 1.0 ? 1.0 / camDist : 1;
            float vfreq =  _VStripeFreq * camDist;
            float vspd = _VStripeSpeed * camDist;
            
            // scan line(vertical)
            float vscan = smoothstep(0.5, 1.0, sin(i.worldPos.y * vfreq + _Time.y * vspd));
            col.rgb += vscan * _Color.rgb * _VStripeIntensity;
            
            // large scan line(vertical)
            col.rgb += smoothstep(-0.5, 1.0, sin(i.worldPos.y * vfreq * 0.025 - _Time.y * 233.335)) * _Color.rgb * 0.25;

            // scan line(horizontal)
            float2 hpos = _HOffset + (i.worldCenter.xz - i.worldPos.xz);
            col.rgb += smoothstep(0.5 * UNITY_PI, UNITY_PI, mod(atan2(hpos.y, hpos.x) * _HStripeFreq * camDist - _Time.y * _HStripeSpeed * camDist, UNITY_PI)) * _Color.rgb * _HStripeIntensity * vscan;
            
            // scatter Edge
            float t = edgeFactor(i.bary.xyz);
            col.rgb += (1-t) * (abs(i.bary.w) > 0.) * 0.5;

            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col;
        }

        fixed4 fragWithBackMask(g2f i) : SV_Target
        {
#ifdef DOUBLESIDE
            return frag(i);
#else
            fixed mask = tex2D(_AlphaMask, i.uv).r;

            clip(mask - _CutOff);

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
            #pragma hull Hull
            #pragma domain Domain
            #pragma geometry geom
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
            #pragma hull Hull
            #pragma domain Domain
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            ENDCG
        }
    }
}
