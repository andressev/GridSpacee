using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataUtilities;
using Unity.VisualScripting;



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

   

    //Costumizable buffers
    public ComputeBuffer neighborhoodBuffer1;
    public ComputeBuffer neighborhoodBuffer2;


    //0 is init, 1 costumizable, 2,3,4... costum settings for cool stuff
    [Range(0,3)]public int kernelToggle=2;
    

    private int[][] randomDataStorage;



    
    
    private RenderTexture startingNoise;


    public void Start()
    {
        int kernel= compute.FindKernel("CSInit");


        randomDataStorage=new int[4][];
        for (int i = 0; i < randomDataStorage.Length; i++)
        {
            randomDataStorage[i] = new int[23];
        }

        //TEXTURE uses inputed noise
        result = AutomataHelper.PictureToRenderTexture(noise);; //width height and bits per pixel
        
        compute.SetInt("width", result.width);
        compute.SetInt("height", result.height);


        compute.SetTexture(kernel, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?

        compute.SetTexture(1, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?
        
        compute.SetTexture(2, "Result", result);



        render.material.SetTexture("_MainTex", result); //Simulation GameObject texture as place to render shader


        compute.Dispatch(kernel, width/8, height/8, 1); //Sends the threads to be processed on the compute shader

        SendRandomData(23,0);
        SendRandomData(23,1);
        SendRandomData(23,2);
        SendRandomData(23,3);
    }

    
    



    // Update is called once per frame


    private int threadRegion;
    void Update()
    {
        
        
        float mouSex=Input.mousePosition.x*((float)noise.width/Screen.width);
        float mouSey= Input.mousePosition.y*(((float)noise.height)/Screen.height);

        //Setting Mouse Pos
        compute.SetFloat("mousePosX", mouSex);
        compute.SetFloat("mousePosY", mouSey);
        compute.SetBool("clickedLeft", Input.GetMouseButton(0));


        
        compute.SetBool("clickedRight", Input.GetMouseButton(1));
        
        
        compute.SetTexture(kernelToggle, "Result", result);
        
        compute.Dispatch(kernelToggle, width / 8, height / 8, 1);


        render.material.SetTexture("_MainTex", result); //We can use result cause in unity whatever you initialize in start you have acces to

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            SendRandomData(23, 0);
            SendRandomData(23, 1);
            SendRandomData(23, 2);
            SendRandomData(23, 3);
           

        }

        threadRegion=((int)Input.mousePosition.x/(int)(Screen.width/2)) + (2*((int)Input.mousePosition.y/(int)(Screen.height/2))); 

        if(Input.GetKeyDown(KeyCode.Tab)){
            MutateRandomData(mutationStrenght,threadRegion);
        }

        
    }
    public void SetNeighborhoodBuffers(List<List<Vector2Int>> neighborhoodCoordinates){
        
        

        kernelToggle= neighborhoodCoordinates.Count-1; //Cause neighbrohood two starts at 1

        Debug.Log("kernel is"+kernelToggle);

        int bufferCount=1; //knows what buffer is being set up

        compute.SetTexture(kernelToggle, "Result", result);

        foreach(List<Vector2Int> neighborhood in neighborhoodCoordinates){
            Vector2Int[] arrayNh= neighborhood.ToArray();
            ComputeBuffer neighborhoodBuffer= ComputeBufferManager.CreateBuffer(arrayNh.Length, sizeof(int)*2);

            neighborhoodBuffer.SetData(arrayNh);

            compute.SetInt($"NeighborhoodBuffer{bufferCount}Size", arrayNh.Length);

            compute.SetBuffer(kernelToggle,$"NeighborhoodBuffer{bufferCount}",neighborhoodBuffer);
            bufferCount++;
        }

        
        

    }



    [Range(1,60)]public int mutationStrenght=20;



    public int[] CreateRandomData(int amountOfInt32){
        int[] randomData= new int [amountOfInt32];

        System.Random rng= new System.Random();

        for(int i=0; i<amountOfInt32; i++){
            randomData[i]= rng.Next();
        }

        return randomData;

    
    }

    public void SendRandomData(int amountOfInt32, int regionIndex){
        int[] randomData=CreateRandomData(amountOfInt32);

        ComputeBuffer randomDataBuffer= ComputeBufferManager.CreateBuffer(amountOfInt32, sizeof(int));

        randomDataBuffer.SetData(randomData);

        compute.SetBuffer(kernelToggle, $"RandomDataBuffer{regionIndex}", randomDataBuffer);

        randomDataStorage[regionIndex]=randomData;

        result = AutomataHelper.PictureToRenderTexture(noise); //width height and bits per pixel
        compute.SetTexture(kernelToggle, "Result", result);



    }
    
    public void MutateRandomData( int mutationStrenght, int baseIndexMutation){
        System.Random rng= new System.Random();

        for(int i=0; i<4; i++){
            if(i==baseIndexMutation){

                continue;
                
            }
            Debug.Log(i);
            for(int pick=0; pick<8; pick++){
                int index= rng.Next(1,12);
                randomDataStorage[i][index]=BitFlip(randomDataStorage[baseIndexMutation][index], mutationStrenght);

            }

            //Mutating update values
            if(rng.Next(1,4)==4){
                randomDataStorage[i][17]=BitFlip(randomDataStorage[baseIndexMutation][17], mutationStrenght/2);
            }


            
            ComputeBuffer randomDataBuffer= ComputeBufferManager.CreateBuffer(23, sizeof(int));


            randomDataBuffer.SetData(randomDataStorage[i]);

            compute.SetBuffer(kernelToggle, $"RandomDataBuffer{i}", randomDataBuffer);

        }
        //Mutating thresholds
        result = AutomataHelper.PictureToRenderTexture(noise); //width height and bits per pixel
        compute.SetTexture(kernelToggle, "Result", result);
        
    }

    public void SetMutation(){
        
    }


    //takes a number and mutation strenght and flips a certain amount of bits depending on mutation strenght
    public int BitFlip(int number, int mutationStrenght){

        int mask= Make27BitMask(mutationStrenght); 

        return number^mask;

    }


    //Given a mutationStrenghtCreates a bit sequence de strenght defines the porbability of fliping each bit
    public int Make27BitMask(int mutationStrenght){

        //mutationStrength should me max 50-60 percent if 100 it will flip back and forth so actually its stronger when 50-60

        System.Random rng= new System.Random();

        int number=0;


        for(int i=1; i<=27; i++){
            if(rng.Next(1,100)<=mutationStrenght){
                number+=(int)Math.Pow(2,i);
            }
        }

        return number;
    }

    public int Make16BitMask(int mutationStrenght){

        //mutationStrength should me max 50-60 percent if 100 it will flip back and forth so actually its stronger when 50-60

        System.Random rng= new System.Random();

        int number=0;


        for(int i=1; i<=16; i++){
            if(rng.Next(1,100)<=mutationStrenght){
                number+=(int)Math.Pow(2,i);
            }
        }

        return number;
    }




    

    

}
