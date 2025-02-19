using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutomataUtilities;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Dynamic;
using UnityEngine.AI;
using System.Runtime.InteropServices;
using System.Globalization;



public class MNCA : MonoBehaviour
{
    // Start is called before the first frame update
   

    
    public ComputeShader compute;

    public float checkAvgs=0;
    [Range(0,15)] public int debuggerVar;
    
    
    private bool usePingBuffer = true;
    public Texture2D noise;

     //render texture that is applied to material

    public Renderer render;

    [Range(0,4)]public int kernelToggle=0;
    

    private int[][] randomDataStorage;
    private Vector2Int[][] ringMatrix;




    //Pingpong necessary variables
    RenderTexture pingBuffer;
    RenderTexture pongBuffer;
    RenderTexture currentBuffer;
    RenderTexture nextBuffer;

    //Mutation strenght
    public GameObject mutationStrengthCounter;
    private Text mutationStrengthText;
    public GameObject fpsCounter;
    private Text fpsText;


    //framestep and pause
    bool frameStepMode=false;
    bool doStep=false;

    

    //slowdown frames
    [Range(0f,1f)]public float updateInterval = 0.1f;
    [Range(0f, 1f)]private float updateIntervalRemap =1f;
    [Range(0f, 1f)] private float displayValue;

    


    ComputeBuffer ParametersBuffer;

    private float timer= 0.0f;

    [Range(1,12)]private int amountOfNhValue = 5; 


    public GameObject amountOfNeighborhoods;


    private ComputeBuffer RandomDataBuffer0;
    private ComputeBuffer RandomDataBuffer1;
    private ComputeBuffer RandomDataBuffer2;
    private ComputeBuffer RandomDataBuffer3;


    public ComputeBuffer[] RandomDataBufferArray;


    public bool mutateNeighborhoods=false;


    public void Start()
    {
        int kernel= compute.FindKernel("CSInit");


        randomDataStorage=new int[4][];
        for (int i = 0; i < randomDataStorage.Length; i++)
        {
            randomDataStorage[i] = new int[23];
        }

        //TEXTURE uses inputed noise

        pingBuffer = AutomataHelper.PictureToRenderTexture(noise); //width height and bits per pixel
        pongBuffer = new RenderTexture(noise.width, noise.height, 24);
        pongBuffer.enableRandomWrite=true;
	    pongBuffer.filterMode=FilterMode.Point;
		pongBuffer.Create();
        
        
        compute.SetInt("width", pingBuffer.width);
        compute.SetInt("height", pingBuffer.height);

        


        

       
        //UI variable text
        mutationStrengthText= mutationStrengthCounter.GetComponent<Text>();
        mutationStrengthText.text=$"{(int)mutationStrength}";

        fpsText= fpsCounter.GetComponent<Text>();
        fpsText.text=$"{(int)updateIntervalRemap}";




        //Sends the preset neighborhood offsets
        Vector2Int[] flatRingMatrix=AutomataHelper.GetFlatRingMatrix();

        ComputeBuffer neighborhoodPreCalc= ComputeBufferManager.CreateBuffer(93*17,sizeof(int)*2);
        neighborhoodPreCalc.SetData(flatRingMatrix);
        compute.SetBuffer(1,"NeighborhoodPreCalc", neighborhoodPreCalc);
        compute.SetBuffer(2,"NeighborhoodPreCalc", neighborhoodPreCalc);
        compute.SetBuffer(3,"NeighborhoodPreCalc", neighborhoodPreCalc);
        compute.SetBuffer(4,"NeighborhoodPreCalc", neighborhoodPreCalc);

       
        uint[] ringSizesArray= {
        1, 8, 12, 16, 24, 28, 36, 40,
        44, 52, 56, 64, 68, 72, 80, 84
        };

        ComputeBuffer ringSizes= ComputeBufferManager.CreateBuffer(16, sizeof(int));
        ringSizes.SetData(ringSizesArray);
        compute.SetBuffer(1,"ringSizes", ringSizes);
        compute.SetBuffer(2,"ringSizes", ringSizes);
        compute.SetBuffer(3,"ringSizes", ringSizes);
        compute.SetBuffer(4,"ringSizes", ringSizes);

        


        //Setting buffers
        RandomDataBuffer0=ComputeBufferManager.CreateBuffer(33,sizeof(int));
        RandomDataBuffer1=ComputeBufferManager.CreateBuffer(33,sizeof(int));
        RandomDataBuffer2=ComputeBufferManager.CreateBuffer(33,sizeof(int));
        RandomDataBuffer3=ComputeBufferManager.CreateBuffer(33,sizeof(int));

        RandomDataBufferArray=new ComputeBuffer[4];

        RandomDataBufferArray[0]=RandomDataBuffer0;
        RandomDataBufferArray[1]=RandomDataBuffer1;
        RandomDataBufferArray[2]=RandomDataBuffer2;
        RandomDataBufferArray[3]=RandomDataBuffer3;


    

    }

    private int threadRegion;
    void Update()
    {
        //Sets shader data --------------------------------------------------------------------------------------------------
        float mouSex = Input.mousePosition.x * ((float)noise.width / Screen.width);
        float mouSey = Input.mousePosition.y * ((float)noise.height / Screen.height);

        // Setting Mouse Position
        compute.SetFloat("mousePosX", mouSex);
        compute.SetFloat("mousePosY", mouSey);
        compute.SetBool("clickedLeft", Input.GetMouseButton(0));
        compute.SetBool("clickedRight", Input.GetMouseButton(1));

        compute.SetFloat("checkAVgs", checkAvgs);
        compute.SetInt("debuggerVar", debuggerVar);
        
        



        //Mutation neighborhoods  toggler
        Text neighborhoodCount=amountOfNeighborhoods.GetComponent<Text>();

        compute.SetInt("counter1", amountOfNhValue);
        compute.SetInt("counter2", amountOfNhValue*8);

        neighborhoodCount.text=$"{amountOfNhValue}";


        

        //HANDLES DATA -----------------------------------------------------------------------------------------------------------------------
        
        // Handle random data on Tab key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < 4; i++)
            {
                SendRandomData(32, i);
            }
        }
        // Calculate the thread region based on mouse position
        threadRegion = ((int)Input.mousePosition.x / (int)(Screen.width / 2)) + (2 * ((int)Input.mousePosition.y / (int)(Screen.height / 2))); 

        // Handle random data mutation on Space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MutateRandomData((int)mutationStrength, threadRegion);
        }



        //HANDLES SHADER--------------------------------------------------------------------------------------------------------------
        timer+=Time.deltaTime;


        //frame step mode, step through frames 
        if(Input.GetKeyDown(KeyCode.F)){
            frameStepMode=!frameStepMode;
        }

        if(Input.GetKeyDown(KeyCode.G)){
            doStep=true;
        }
        

        //increase - decrease framrate and dispatch shader-----------------------------------------------------------
        if(updateInterval<=1 && updateInterval>=0.0){

            if(Input.GetKey(KeyCode.UpArrow)) updateInterval-=0.0007f;
            if(Input.GetKey(KeyCode.DownArrow)) updateInterval+=0.0007f;
        }else if(updateInterval>1){
            updateInterval=1;
        }else if(updateInterval<0){
            updateInterval=0;
        }

        
        
        if (updateInterval == 0.0f || timer >= updateInterval)
        {
            if(!frameStepMode || doStep==true){
                doStep=false;

                currentBuffer = usePingBuffer ? pingBuffer : pongBuffer;
                nextBuffer    = usePingBuffer ? pongBuffer : pingBuffer;

                compute.SetTexture(kernelToggle, "Input", currentBuffer);
                compute.SetTexture(kernelToggle, "Result", nextBuffer);

            
                compute.Dispatch(kernelToggle, currentBuffer.width / 8, currentBuffer.height / 8, 1);
                
                
                // Swap buffers
                usePingBuffer = !usePingBuffer;
                timer = 0.0f;
            }
            
            
        }


        //TOGGLE flash funcitonality
        if(Input.GetKey(KeyCode.Alpha1)){

            render.material.SetTexture("_MainTex", pingBuffer);
        }else if(Input.GetKey(KeyCode.Alpha2)){

            render.material.SetTexture("_MainTex", pongBuffer);
        }else{

            render.material.SetTexture("_MainTex", nextBuffer);
        }

    

        //SAVE mutation data
        if(Input.GetKeyDown(KeyCode.S)){
            SaveAutomaton();
        }

      
        // Adjust mutation strength based on horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0 && mutationStrength <= 60)
        {
            mutationStrength += 15 * Time.deltaTime * horizontalInput;
        }
        else if (horizontalInput < 0 && mutationStrength >= 0)
        {
            mutationStrength -= 15 * Time.deltaTime * -horizontalInput;
        }



        // Update UI text
        displayValue=updateIntervalRemap-updateInterval;
        mutationStrengthText.text = $"{(int)mutationStrength}";
        fpsText.text=$"{displayValue:0.0}";
    }
    public void SetNeighborhoodBuffers(List<List<Vector2Int>> neighborhoodCoordinates){
        
        

        kernelToggle= neighborhoodCoordinates.Count-1; //Cause neighbrohood two starts at 1

        
        for (int i = 0; i < 4; i++){
            SendRandomData(32, i);
        }


        Debug.Log("kernel is"+kernelToggle);

        int bufferCount=1; //knows what buffer is being set up

        // currentBuffer = usePingBuffer ? pingBuffer : pongBuffer;
        // compute.SetTexture(kernelToggle, "Input", currentBuffer);
        pingBuffer=AutomataHelper.PictureToRenderTexture(noise);
        pongBuffer=AutomataHelper.PictureToRenderTexture(noise);

        foreach(List<Vector2Int> neighborhood in neighborhoodCoordinates){
            Vector2Int[] arrayNh= neighborhood.ToArray();
            ComputeBuffer neighborhoodBuffer= ComputeBufferManager.CreateBuffer(arrayNh.Length, sizeof(int)*2);

            neighborhoodBuffer.SetData(arrayNh);

            compute.SetInt($"NeighborhoodBuffer{bufferCount}Size", arrayNh.Length);

            compute.SetBuffer(kernelToggle,$"NeighborhoodBuffer{bufferCount}",neighborhoodBuffer);
            bufferCount++;
        }

        
        

    }



    [Range(1,60)]public float mutationStrength=20;
    
    public int[] CreateRandomData(int amountOfInt32){
        int[] randomData= new int [amountOfInt32+1];

        System.Random rng= new System.Random();

        for(int i=0; i<amountOfInt32; i++){
            randomData[i]= rng.Next();
        }
        randomData[amountOfInt32]=amountOfNhValue;

        return randomData;

    
    }

    public void SendRandomData(int amountOfInt32, int regionIndex){
        int[] randomData=CreateRandomData(amountOfInt32);

        

        RandomDataBufferArray[regionIndex].SetData(randomData);

        compute.SetBuffer(kernelToggle, $"RandomDataBuffer{regionIndex}", RandomDataBufferArray[regionIndex]);
        

        randomDataStorage[regionIndex]=randomData;


        ReSeed();

       
    }
    
    public void MutateRandomData( int mutationStrength, int baseIndexMutation){
        System.Random rng= new System.Random();

        for(int i=0; i<4; i++){
            if(i==baseIndexMutation){

                continue;
                
            }
            
            randomDataStorage[i]=(int[])randomDataStorage[baseIndexMutation].Clone();
            for(int pick=0; pick<amountOfNhValue*2; pick++){
                int index= rng.Next(1,amountOfNhValue*2);
                randomDataStorage[i][index]=BitFlip(randomDataStorage[baseIndexMutation][index], mutationStrength);

            }
            if(mutationStrength>10){
                if(rng.Next(1,4)==4){
                    randomDataStorage[i][17]=BitFlip(randomDataStorage[baseIndexMutation][17], mutationStrength/2);
                }

            }

            if(mutateNeighborhoods){
                

                for(int number=0; number<4; number++ ){
                    int index=rng.Next(24,27);
                    randomDataStorage[i][index]=BitFlip(randomDataStorage[baseIndexMutation][index], mutationStrength);
                }
                
            }
            
            





            


            RandomDataBufferArray[i].SetData(randomDataStorage[i]);

            compute.SetBuffer(kernelToggle, $"RandomDataBuffer{i}", RandomDataBufferArray[i]);



            

        }
        
        


       ReSeed();
        
    }

    //Re-seeds screem
    public void ReSeed(){
        pingBuffer=AutomataHelper.PictureToRenderTexture(noise);
        pongBuffer=AutomataHelper.PictureToRenderTexture(noise);
    }

    //takes a number and mutation strenght and flips a certain amount of bits depending on mutation strenght
    public int BitFlip(int number, int mutationStrength){

        int mask= Make27BitMask(mutationStrength); 

        return number^mask;

    }


    //Given a mutationStrengthCreates a bit sequence de strenght defines the porbability of fliping each bit
    public int Make27BitMask(int mutationStrength){

        //mutationStrength should me max 50-60 percent if 100 it will flip back and forth so actually its stronger when 50-60

        System.Random rng= new System.Random();

        int number=0;


        for(int i=1; i<=27; i++){
            if(rng.Next(1,100)<=mutationStrength){
                number |= (1 << i); //or operation with the one displaced i spots to the left, so it will turn it on
            }
        }

        return number;
    }


    //Given a mutationStrengthCreates a 16 bit sequence de strenght defines the porbability of fliping each bit
    public int Make16BitMask(int mutationStrength){

        //mutationStrength should me max 50-60 percent if 100 it will flip back and forth so actually its stronger when 50-60

        System.Random rng= new System.Random();

        int number=0;


        for(int i=1; i<=16; i++){
            if(rng.Next(1,100)<=mutationStrength){
               number |= (1 << i);
            }
        }

        return number;
    }


    public void AddToAmountOfNeighborhoods(){
        if(amountOfNhValue<12){
            amountOfNhValue++;
            for(int i=0; i<4; i++){
                randomDataStorage[i][32]=amountOfNhValue;
            }
        }
        
    }

    public void SubtractFromAmountOfNeighborhoods(){
        if(amountOfNhValue>1){
            amountOfNhValue--;
            for(int i=0; i<4; i++){
                randomDataStorage[i][32]=amountOfNhValue;
            }
        }
    }


    public void toggleMutateNeighborhoods(){
        
        mutateNeighborhoods=!mutateNeighborhoods;
        
    }



    //Copies to clipboard the sequence of the automaton
    public string SaveAutomaton(){
        int[] sequence=randomDataStorage[threadRegion];

        string sequenceString="";


        
        for(int i=0; i<sequence.Length; i++){
            sequenceString+=sequence[i]+",";
        }

        Debug.Log(sequenceString);
        AutomataHelper.CopyToClipboard(sequenceString);
        return sequenceString;
    }

    public void SetAutomaton(string sequenceString){
        Debug.Log(sequenceString);
        string[] parameterStr=sequenceString.Split(",");

        int[] parameters = new int[33];

        kernelToggle=4;



        for(int i=0; i<33; i++){
            Debug.Log(i);
            if (int.TryParse(parameterStr[i], out int result)){
                parameters[i] = result;
            }
            
        }
        

        amountOfNhValue=parameters[32];

        for(int i=0; i<4; i++){
            randomDataStorage[i]=parameters;
            RandomDataBufferArray[i].SetData(randomDataStorage[i]);
            compute.SetBuffer(kernelToggle, $"RandomDataBuffer{i}", RandomDataBufferArray[i]);
        }

        ReSeed();
    }

    void OnApplicationQuit(){
        foreach(ComputeBuffer buffer in RandomDataBufferArray){
            buffer.Dispose();
        }
       

    }
}
