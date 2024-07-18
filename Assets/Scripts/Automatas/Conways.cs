using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conways : MonoBehaviour
{
    

    public int width = 512;
    public int height = 512;


    
    public ComputeShader compute;
    public RenderTexture result;

     //render texture that is applied to material

    public Renderer render;


    // Start is called before the first frame update
    void Start()
    {

        
        int kernel= compute.FindKernel("CSInit");


        //TEXTURE
        result = new RenderTexture(width, height, 24); //width height and bits per pixel
        result.enableRandomWrite= true; //Possible to edit the texture on runtime?
        result.filterMode = FilterMode.Point; //SO it doesnt interpolate pixels
        result.Create(); //Creates the render texure lol
    

        
        
        compute.SetTexture(kernel, "Result", result); //Variable result in compute is assigned. Is this how we can pass buffers?

        compute.Dispatch(kernel, width/8, height/8, 1); //Sends the threads to be processed on the compute shader

        render.material.SetTexture("_MainTex", result); //Aqui ponemos como la textura del quad nuestro output del shader.

       
        
    }

    // Update is called once per frame
    void Update()
    {
       int updateKernel = compute.FindKernel("CSUpdateConway"); //We use now the other pragma function


       compute.SetTexture(updateKernel, "Result", result);
       compute.Dispatch(updateKernel, width / 8, height / 8, 1);


       render.material.SetTexture("_MainTex", result); //We can use result cause in unity whatever you initialize in start you have acces to


    }
}
