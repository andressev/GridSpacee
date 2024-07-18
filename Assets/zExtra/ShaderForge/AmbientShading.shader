Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        //Aqui se puede definir propiedades que se contorlan desde unity :)
        _Color("Color", Color) = (1,1,1,0)
        _Gloss("Gloss", float)= 1
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            //Instanciamos vert y frag que entiendo que es un tipo de clase/struct? Donde ver es el vertex shader
            #pragma vertex vert
            #pragma fragment frag
            

            //Libreria que usaremos
            #include "UnityCG.cginc"
            #include "Lighting.cginc"            
            #include "AutoLight.cginc"

            
            //Datos del mesh: position, normal's, UV's(No entiendo bien todavia ), tangents, colors 
            struct VertexInput
            {
                float4 vertex : POSITION; 
                float3 normal : NORMAL; 
                // float2 uv0 : TEXTCOORD0;
                
            };

            
            //El vertex shader se compone de dos estructuras un input y un output
            struct VertexOutput
            {
                
                float4 clipSpacePos : SV_POSITION;
                // float2 uv0 : TEXTCOORD0;
                float3 normal : TEXTCOORD1;
                float3 worldPos : TEXTCOORD2;
            };
            
            
            //Esto ni idea todavia
            //sampler2D _MainTex;
            // float4 _MainTex_ST;
            float4 _Color;
            float _Gloss;
            
            /*vert es el vertex shader donde definimos que regresaremos un vertexOutput tomando un vertexInput, 
            donde simplemente asignamos los valores.*/
            
            VertexOutput vert(VertexInput v)
            {
                /*Aquí están los interpoladores, que obtienen los datos para los fragmentos que no sabemos su normal, 
                esto se hace interpolando entre los valores mas cercanos, por los interpoladores terminamos con
                datos continuos enves de discretos.
                */
                VertexOutput o;
                // o.uv0 = v.uv0;
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            //Una funcion que divide un rango continuo en diferentes partes iguales (steps)
            float3 Posterize(float steps, float value )
            {
                    return floor(value*steps)/steps;
            }
                


            //fragment shader donde ocurren las transf :)
            float4 frag (VertexOutput i) : SV_Target
            {
                
                float3 normal= normalize(i.normal);//Como i.normal esta interpolado lo normalizamos para que todos nuestros vectores valgan 1.
                //Lighting


                
                
                //Direct diffuse light
                float3 lightDirection= _WorldSpaceLightPos0.xyz; //De donde viene la luz
                float3 lightColor= _LightColor0.rgb;   //Color de la luz
                float gradient= max(0,dot(normal, lightDirection)); //En referencia a donde viene la luz que tan "parecidos son los vec"
                // gradient= Posterize(3, gradient);
                float3 diffuseLight= lightColor*gradient;  //el color por el gradiente para cambiar el color de la luz
                //Ambient light
                float3 ambientLight= float3(0.1,0.2,0.3); //le damos una luz inicial que cubre todo el objeto uniformemente


                //Direct specular light (PHONG)
                float3 camPos = _WorldSpaceCameraPos; //Position de la camara
                float3 fragToCam = camPos-i.worldPos; //Direccion del vector  que sale del fragmento hacia la camara. Resta de vectores. 
                float3 viewDir = normalize(fragToCam);
                float3 viewerReflection = reflect(-viewDir, normal);//Es el vector que refleja en la superficie del objeto cuando un rayo sale de la camara
                float3 specularLight= max(0,dot(viewerReflection,lightDirection)); 
                specularLight= pow(specularLight, _Gloss);//Elevar a una potencia entre mas grande son pocos los valores que se acercan a 1 y crecen si los elevas
                // specularLight= Posterize(4, specularLight); //Anyt
                float3 directSpecular=specularLight*lightColor;
                //Blinn-Phong
                

                //Composite
                float3 finalOutput=(ambientLight+diffuseLight)*_Color; //Valor que da la iluminacion Multiplicams por el color de nuestro  material.
                finalOutput+=directSpecular;

                return float4(finalOutput,0); 
            }   
            ENDCG
        }
    }
}
