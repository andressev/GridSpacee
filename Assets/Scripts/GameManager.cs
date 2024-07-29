using System.Collections;
using System.Collections.Generic;
using System.Threading;
// using ComputeShaderUtility;
using UnityEngine;
using AutomataUtilities;
using UnityEditor;

using UnityEngine.UI;
using UnityEngine.Device;
using Unity.VisualScripting;
using UnityEngine.Rendering;


public class GameManager : MonoBehaviour
{
    

    //UI 
    public GameObject UI;
    public GameObject grid;
    public GameObject gridUI;
    public GameObject pickUI;
    public Transform nhGrid;
    public GameObject nhPrefab;


   

    public Camera myCamera;


    public GameObject menuUI;

    public  List<Neighborhood> neighborhoodCollection;
    private GridGenerator gridGen;


    public int width;
    public int height;



    // private List<Neighborhood> neighborhoodElection; 

    

    
    void Start()
    {
        neighborhoodCollection=new List<Neighborhood>();
        gridGen=grid.GetComponent<GridGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    //-----UI Managment------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------
    
    public void GetMenu(){

        gridUI.SetActive(false);
        pickUI.SetActive(false);
        menuUI.SetActive(true);

    }
    public void OpenGridEditor(){
        menuUI.SetActive(false);
        gridUI.SetActive(true);
    }

    public void OpenPicker(){
        menuUI.SetActive(false);
        pickUI.SetActive(true);
    }


    public void ResetNeighborhood(){
        gridGen.GenerateGrid();
    }

    public void ToggleGrid(){
        if(UI.activeSelf){
            UI.SetActive(false);
        }else{
            UI.SetActive(true);
        }
        
    }



    //----------------------------------------------------------------------------------------------------------------------------------
    //-----Neighbrohood Editor and picker interaction-----------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------
    

    //Note for my scren width 1940x 1600 works
    //For QHD 1950 x 1420 works


    //Save neighborhood button
    public void SaveNeighborhood(){
        Debug.Log("save button click");
        
        List<Vector2Int> coordinates=new List<Vector2Int>(gridGen.neighbors); //Makes a copy of the neighbors


        if(coordinates!= null){
            Debug.Log("Working svae");

            //Fetching variables need to figure out how to get them before
            // ScreenCapture.CaptureScreenshot( "yapu.png");
            
            Neighborhood nh= new Neighborhood(coordinates); 

           
            StartCoroutine(TakeScreenshot(width,height, myCamera, nh));

            //Not sure if i want to acces the shaderManager here or later.
            // MNCA automataHandler= simulation.GetComponent<MNCA>();
            // automataHandler.SetNh(nh);  

            
            
            
            
            //Done in the coroutine
            // gridGen.GenerateGrid();//Reset grid
            
            Debug.Log(coordinates.Count);


        }
        
    }
    IEnumerator TakeScreenshot(int width, int height, Camera myCamera, Neighborhood nh){

			yield return new WaitForEndOfFrame();
			myCamera.targetTexture= RenderTexture.GetTemporary(width, height, 16);
		
			

			RenderTexture renderTexture=myCamera.targetTexture;
			Texture2D result= new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
			Rect rect       =  new Rect(0,0, renderTexture.width, renderTexture.height);
			result.ReadPixels(rect, 0,0);
            result.Apply();
			
			nh.screenshot=result;


			RenderTexture.ReleaseTemporary(renderTexture);
			myCamera.targetTexture=null;
            myCamera.Render();

            neighborhoodCollection.Add(nh);

            AddNeighborhoodToPicker(nh);

            gridGen.GenerateGrid();//Reset grid

			Debug.Log("Screenshot Taken");
	}


    public Transform pickedNeighborhoods;
    public void ResetPickedNeighboorhood(){
        foreach(Transform child in pickedNeighborhoods){
            Destroy(child.gameObject);
        }

    }

    public GameObject simulation;
    public void ApplyNeighborhoods(){
        MNCA mnca= simulation.GetComponent<MNCA>();

        List<List<Vector2Int>> neighborhoodCoordinates= new List<List<Vector2Int>>();

        foreach(Transform child in pickedNeighborhoods.transform){
            NeighborhoodTemplate infoTemplate =child.GetComponent<NeighborhoodTemplate>();
      
            List<Vector2Int> neighborhood= infoTemplate.nh.coordinates;

            neighborhoodCoordinates.Add(neighborhood);

        }
        //Setting the mnca buufers
        mnca.SetNeighborhoodBuffers(neighborhoodCoordinates);
        mnca.Start();


    }

    
    


    private Image image;
    private NeighborhoodTemplate infoPass;
    public void  AddNeighborhoodToPicker(Neighborhood nh){

        Debug.Log("Starting to add");

        GameObject neighborhood= Instantiate(nhPrefab, nhGrid);
        image = neighborhood.GetComponent<Image>();
        image.sprite= Sprite.Create(nh.screenshot,new Rect(0,0,nh.screenshot.width, nh.screenshot.height), new Vector2(0.5f, 0.5f));

        infoPass= neighborhood.GetComponent<NeighborhoodTemplate>();
        infoPass.nh=nh;



        Debug.Log("added with amount of coords: "+infoPass.nh.coordinates.Count );
    }



   


}
