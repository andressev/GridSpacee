using System;
using System.Collections;
using System.Collections.Generic;
using ComputeShaderUtility;
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


        //TEXTURE
        result = AutomataHelper.PictureToRenderTexture(noise); //width height and bits per pixel
        // result.enableRandomWrite= true; //Possible to edit the texture on runtime?
        // result.filterMode = FilterMode.Point; //SO it doesnt interpolate pixels
        //result.Create(); //Creates the render texure lol
    

        
        
        compute.SetTexture(kernel, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?
        compute.SetTexture(3, "Result", result);
        compute.SetTexture(1, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?


        render.material.SetTexture("_MainTex", result); //Aqui ponemos como la textura del quad nuestro output del shader.


      

        //SQUARE BUFFER 
        Vector2Int[] squareOffsets = ComputeHelper.GenerateGrid(5);
        ComputeHelper.Release(squareBuffer);
        squareBuffer = new ComputeBuffer(squareOffsets.Length, sizeof(int)*2);
        squareBuffer.SetData(squareOffsets);


        compute.SetBuffer(3, "SquareBuffer", squareBuffer);

      
        compute.Dispatch(kernel, width/8, height/8, 1); //Sends the threads to be processed on the compute shader

        
    }

    
    public float slider0, slider1, slider2, slider3, slider4, slider5, slider6, slider7, slider8, slider9, slider10, slider11;

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
        

        

        

        //Setting Mouse Pos
        compute.SetFloat("mousePosX", Input.mousePosition.x*((float)width/Screen.width)%width);
        compute.SetFloat("mousePosY", Input.mousePosition.y*((float)height/Screen.height)%height);
        compute.SetBool("clickedLeft", Input.GetMouseButton(0));
        compute.SetBool("clickedRight", Input.GetMouseButton(1));
        
        
        compute.SetTexture(kernelToggle, "Result", result);
        
        compute.Dispatch(kernelToggle, width / 8, height / 8, 1);


        render.material.SetTexture("_MainTex", result); //We can use result cause in unity whatever you initialize in start you have acces to


    }
    public void SetNeighborhoodBuffers(List<Vector2Int> neighborhood1, List<Vector2Int> neighborhood2){
        
        Vector2Int[] nh1= neighborhood1.ToArray();
        Vector2Int[] nh2= neighborhood2.ToArray();

        int kernelCustomizable= compute.FindKernel("Customizable");

        Debug.Log("kernel is"+kernelCustomizable);

        // neighborhoodBuffer1.Release();
        // neighborhoodBuffer2.Release();

        neighborhoodBuffer1= new ComputeBuffer(nh1.Length, sizeof(int)*2);
        neighborhoodBuffer2= new ComputeBuffer(nh2.Length, sizeof(int)*2);

        compute.SetTexture(1, "Result", result);
        

        neighborhoodBuffer1.SetData(nh1);
        neighborhoodBuffer2.SetData(nh2);

        compute.SetInt("NeighborhoodBuffer1Size", nh1.Length);
        compute.SetInt("NeighborhoodBuffer2Size", nh2.Length);



        compute.SetBuffer(kernelCustomizable,"NeighborhoodBuffer1",neighborhoodBuffer1);
        compute.SetBuffer(kernelCustomizable,"NeighborhoodBuffer2",neighborhoodBuffer2);

        Start();
        

    }

    public void SetSliders(){
        List<float> sliderValues=sliderManager.GetComponent<SliderBehavior>().sliderValues;


        for(int i=0; i<sliderValues.Count; i++){
            compute.SetFloat($"slider{i}", sliderValues[i]);
        }

        Debug.Log(sliderValues[0]);
    }

    

}
