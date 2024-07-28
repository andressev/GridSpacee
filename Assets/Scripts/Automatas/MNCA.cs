using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataUtilities;



public class MNCA : MonoBehaviour
{
    // Start is called before the first frame update
    public int width = 512;
    public int height = 512;


    
    public ComputeShader compute;
    public RenderTexture result;
    public Texture2D noise;

     //render texture that is applied to material

    public Renderer render;

    public ComputeBuffer offsetBuffer;
    public ComputeBuffer squareBuffer;

    

    //Costumizable buffers
    public ComputeBuffer neighborhoodBuffer1;
    public ComputeBuffer neighborhoodBuffer2;


    //0 is init, 1 costumizable, 2,3,4... costum settings for cool stuff
    public int kernelToggle;
    


    //Compute Shader Dynamic variables
    
    

    //Sets neighboorhoods at runtime


    public void Start()
    {
        int kernel= compute.FindKernel("CSInit");


        //TEXTURE uses inputed noise
        result = AutomataHelper.PictureToRenderTexture(noise); //width height and bits per pixel
        
        compute.SetTexture(kernel, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?

        compute.SetTexture(1, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?

        compute.SetTexture(2, "Result", result);



        render.material.SetTexture("_MainTex", result); //Simulation GameObject texture as place to render shader


        compute.Dispatch(kernel, width/8, height/8, 1); //Sends the threads to be processed on the compute shader

        
    }

    
    public float slider0, slider1, slider2, slider3, slider4, slider5, slider6, slider7, slider8, slider9, slider10, slider11;
    public int slider12;

    public GameObject sliderManager;


    // Update is called once per frame
    void Update()
    {
        // int kernelUpdate = compute.FindKernel("CSBugs");

        //SETTING VARIABLES
        compute.SetFloat("slider0", slider0/100);
        compute.SetFloat("slider1", slider1/100);
        compute.SetFloat("slider2", slider2/100);
        compute.SetFloat("slider3", slider3/100);
        compute.SetFloat("slider4", slider4/100);
        compute.SetFloat("slider5", slider5/100);
        compute.SetFloat("slider6", slider6/100);
        compute.SetFloat("slider7", slider7/100);
        compute.SetFloat("slider8", slider8/100);
        compute.SetFloat("slider9", slider9/100);
        compute.SetFloat("slider10", slider10/100);
        compute.SetFloat("slider11", slider11/100);
        compute.SetInt("slider12", slider12);

        
        
        float mouSex=Input.mousePosition.x*((float)noise.width/Screen.width);
        float mouSey= Input.mousePosition.y*(((float)noise.height)/Screen.height);

        //Setting Mouse Pos
        compute.SetFloat("mousePosX", mouSex);
        compute.SetFloat("mousePosY", mouSey);
        compute.SetBool("clickedLeft", Input.GetMouseButton(0));


        if(Input.GetMouseButton(0)){
            Debug.Log($"{mouSex}, {mouSey}");
        }
        compute.SetBool("clickedRight", Input.GetMouseButton(1));
        
        
        compute.SetTexture(kernelToggle, "Result", result);
        
        compute.Dispatch(kernelToggle, width / 8, height / 8, 1);


        render.material.SetTexture("_MainTex", result); //We can use result cause in unity whatever you initialize in start you have acces to


    }
    public void SetNeighborhoodBuffers(List<List<Vector2Int>> neighborhoodCoordinates){
        
        

        int kernelCustomizable= compute.FindKernel("TwoNeighborhoods");

        Debug.Log("kernel is"+kernelCustomizable);

        int bufferCount=1; //knows what buffer is being set up

        compute.SetTexture(1, "Result", result);

        foreach(List<Vector2Int> neighborhood in neighborhoodCoordinates){
            Vector2Int[] arrayNh= neighborhood.ToArray();
            ComputeBuffer neighborhoodBuffer= new ComputeBuffer(arrayNh.Length, sizeof(int)*2);

            neighborhoodBuffer.SetData(arrayNh);

            compute.SetInt($"NeighborhoodBuffer{bufferCount}Size", arrayNh.Length);

            compute.SetBuffer(kernelCustomizable,$"NeighborhoodBuffer{bufferCount}",neighborhoodBuffer);
            bufferCount++;
        }

        
        

    }
   


    

    

}
