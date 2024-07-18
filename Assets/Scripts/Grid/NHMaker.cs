using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using ComputeShaderUtility;

public class NHMaker : MonoBehaviour
{
    
    public RenderTexture result;
    public Renderer render;

    public ComputeShader compute; 

    public ComputeBuffer circleBuffer;

    public int size = 21;

    
    Vector2Int [] GenerateCircleOffsets(int radius)
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        
        int x = radius;
        int y = 0;
        int decisionOver2 = 1 - x;   // Decision criterion divided by 2 evaluated at x=r, y=0

        while (y <= x)
        {
            offsets.Add(new Vector2Int( x,  y));
            offsets.Add(new Vector2Int( y,  x));
            offsets.Add(new Vector2Int(-x,  y));
            offsets.Add(new Vector2Int(-y,  x));
            offsets.Add(new Vector2Int(-x, -y));
            offsets.Add(new Vector2Int(-y, -x));
            offsets.Add(new Vector2Int( x, -y));
            offsets.Add(new Vector2Int( y, -x));
            y++;
            if (decisionOver2 <= 0)
            {
                decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
            }
            else
            {
                x--;
                decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
            }
        }

        Vector2Int[] offsetArray= new Vector2Int[offsets.Count];
        for(int i=0; i<offsets.Count; i++){
            offsetArray[i]=offsets[i];
        }

        return offsetArray;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HELLO");
        //Calling shader function 
        int kernel = compute.FindKernel("CSCircleMaker");


        //BUFFER pixelGrid 
        result= new RenderTexture(size, size, 24);
        result.enableRandomWrite= true; 
        result.filterMode= FilterMode.Point; //Sharp pixels
        result.Create();

        //BUFFER assign
        compute.SetTexture(kernel, "Result", result);

        
        //Displaying material on quad
        render.material.SetTexture("_MainTex", result);



        //BUFFER Creation
        Vector2Int[] circleOffsets  = GenerateCircleOffsets(5);
        circleBuffer = new ComputeBuffer(circleOffsets.Length, sizeof(int)*2); //Generate compute buffer, size of the count, 2bit integer size
        circleBuffer.SetData(circleOffsets);
        //BUFFER assign
        compute.SetBuffer(0, "CircleBuffer", circleBuffer);
        //BUFFER Status
        Debug.Log("circleBuffer working:" +circleBuffer.IsValid());        



        //Run compute shader, on kernel, assigns to each pixel an id on a 1D matrix size/8 X size/8 
        compute.Dispatch(kernel, 20,20 , 1);


        Debug.Log("Size of array:" +circleOffsets.Length);   

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
