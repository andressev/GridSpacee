namespace AutomataUtilities{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    // using ComputeShaderUtility;
    using UnityEngine;
    
    public struct Neighborhood{

		public List<Vector2Int> coordinates;
		public Texture2D screenshot;
		

		// public Neighborhood(List<Vector2Int> coordinates, RenderTexture screenshot, string name){
		// 	this.coordinates=coordinates;
		// 	this.screenshot=screenshot;
		// 	this.name=name;
		// }
		

		public Neighborhood(List<Vector2Int> coordinates){
			this.coordinates=coordinates;
			this.screenshot=null;
			
		}

		public String PrintCoords(){
			

			return coordinates.ToString();;
		}


    };


	public static class AutomataHelper{

		public static Vector2Int[] listToArray(List<Vector2Int> list){
				Vector2Int[] arr = new Vector2Int[list.Count];


				for(int i=0; i<list.Count; i++){
					arr[i]=list[i];
				}

				return arr;
			}
		public static IEnumerator TakeScreenshot(int width, int height, Camera myCamera, Neighborhood nh){

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

				

			Debug.Log("Screenshot Taken");




		}
		public static RenderTexture PictureToRenderTexture(Texture2D tex){

			RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 24);
			Debug.Log(tex.width+",  "+ tex.height);
			renderTexture.enableRandomWrite=true;
			renderTexture.filterMode=FilterMode.Point;
			renderTexture.Create();

				// Set the RenderTexture as the active render target
			RenderTexture activeRenderTexture = RenderTexture.active;
			RenderTexture.active = renderTexture;

				// Copy the source texture into the RenderTexture using Graphics.Blit
			Graphics.Blit(tex, renderTexture);

			// Restore the active render texture
			RenderTexture.active = activeRenderTexture;

			return renderTexture;
		}
		public static double Norm(Vector2Int vec){
			double x= (double) vec.x;
        	double y= (double) vec.y;

        	return Math.Sqrt(x*x+y*y);
		}
	} 
	
	
	

    
}