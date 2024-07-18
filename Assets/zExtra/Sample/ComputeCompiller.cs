using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeCompiller : MonoBehaviour
{
    // Start is called before the first frame update
    public ComputeShader compute;
    public RenderTexture result; 

    public float color;
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        int kernel = compute.FindKernel("CSMain");

        result = new RenderTexture(512, 512, 24); 
        result.enableRandomWrite= true;

        result.Create();

        compute.SetFloat("var", color);
        compute.SetTexture(kernel, "Result", result); 
        compute.Dispatch(kernel, 512/8, 512/8, 1);
    }
}
