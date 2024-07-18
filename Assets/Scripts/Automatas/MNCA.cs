using System;
using System.Collections;
using System.Collections.Generic;
using ComputeShaderUtility;
using UnityEngine;

public class MNCA : MonoBehaviour
{
    // Start is called before the first frame update
    public int width = 512;
    public int height = 512;


    
    public ComputeShader compute;
    public RenderTexture result;

     //render texture that is applied to material

    public Renderer render;

    public ComputeBuffer offsetBuffer;
    public ComputeBuffer squareBuffer;


    //Compute Shader Dynamic variables
    public float range1,range2,range3;




    void Start()
    {
        int kernel= compute.FindKernel("CSInit");


        //TEXTURE
        result = new RenderTexture(width, height, 24); //width height and bits per pixel
        result.enableRandomWrite= true; //Possible to edit the texture on runtime?
        result.filterMode = FilterMode.Point; //SO it doesnt interpolate pixels
        result.Create(); //Creates the render texure lol
    

        
        
        compute.SetTexture(kernel, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?
        compute.SetTexture(3, "Result", result);

        render.material.SetTexture("_MainTex", result); //Aqui ponemos como la textura del quad nuestro output del shader.


        //BUFFER
        //BUFFER Creation
        Vector2Int[] circleOffsets  = ComputeHelper.GenerateRings(7,4);
        ComputeHelper.Release(offsetBuffer);
        offsetBuffer = new ComputeBuffer(circleOffsets.Length, sizeof(int)*2); //Generate compute buffer, size of the count, 2bit integer size
        offsetBuffer.SetData(circleOffsets);
        //BUFFER assign
        
        compute.SetBuffer(1,"CircleBuffer", offsetBuffer);
       

        //SQUARE BUFFER 
        Vector2Int[] squareOffsets = ComputeHelper.GenerateGrid(5);
        ComputeHelper.Release(squareBuffer);
        squareBuffer = new ComputeBuffer(squareOffsets.Length, sizeof(int)*2);
        squareBuffer.SetData(squareOffsets);


        compute.SetBuffer(3, "SquareBuffer", squareBuffer);



        //BUFFER Status
        Debug.Log("circleBuffer working:" +offsetBuffer.IsValid());       
        
        compute.Dispatch(kernel, width/8, height/8, 1); //Sends the threads to be processed on the compute shader

        //Setting BUFFER size
        compute.SetInt("offsetSize", circleOffsets.Length);


    }

    // Update is called once per frame
    void Update()
    {
        int kernelUpdate = compute.FindKernel("CSBugs");

        //SETTING VARIABLES
        compute.SetFloat("range1", range1);
        compute.SetFloat("range2", range2);
        compute.SetFloat("range3", range3);
        
        compute.SetTexture(kernelUpdate, "Result", result);
        
        compute.Dispatch(kernelUpdate, width / 8, height / 8, 1);


        render.material.SetTexture("_MainTex", result); //We can use result cause in unity whatever you initialize in start you have acces to

       

    }
}
