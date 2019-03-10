Shader "Custom/Float Tex" {
    Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
	} 
    SubShader {
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            #include "Lighting.cginc"
            
	        fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

            struct a2v{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
            };

            struct v2f {
 	            float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v){
            	v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            float2 floating(float2 uv){
                uv.x +=  _Time.x/6;
                uv.y +=  _Time.x/6;
                return uv;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed3 worldNormal = normalize(i.worldNormal);
			    fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				
				fixed4 texColor = tex2D(_MainTex, floating(i.uv));
				
				fixed3 albedo = texColor.rgb * _Color.rgb;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
				
				return fixed4(ambient + diffuse, 1.0);
            }

            ENDCG
        }
    }   
    FallBack "Diffuse"
}