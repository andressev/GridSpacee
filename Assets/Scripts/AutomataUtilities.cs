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

	

	public class ComputeBufferManager : MonoBehaviour{
		public static List<ComputeBuffer> buffers = new List<ComputeBuffer>();

		public static ComputeBuffer CreateBuffer(int count, int stride)
		{
			ComputeBuffer buffer = new ComputeBuffer(count, stride);
			buffers.Add(buffer);

			
			return buffer;
		}

		public static ComputeBuffer CreateBufferConstant(int count, int stride, ComputeBufferType type){
			ComputeBuffer buffer = new ComputeBuffer(count, stride, type);
			buffers.Add(buffer);

			
			return buffer;
		}

		public static void DisposeBuffer(ComputeBuffer buffer)
		{
			if (buffer != null)
			{
				buffer.Dispose();
				buffers.Remove(buffer);
			}
		}

		void OnApplicationQuit()
		{
			DisposeAllBuffers();
		}

		public static void DisposeAllBuffers()
		{
			foreach (var buffer in buffers)
			{
				if (buffer != null)
				{
					buffer.Dispose();
				}
			}
			buffers.Clear();
		}
		public static void DisposeHalfOfBuffers(){
			for(int i=0; i<20; i++){
				DisposeBuffer(buffers[i]);

			}
		}
	}

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

		public static double Norm(Vector2 vec){
			double x= (double) vec.x;
        	double y= (double) vec.y;


        	return Math.Sqrt(x*x+y*y);

			
		}
		public static Vector2Int[] GenerateRingCoords(int radius, Vector2Int[] ring) {
			int auxPoint = 1 - radius;
			int count = 0;

			if (radius == 0) {
				ring[0] = new Vector2Int(0, 0);
				ring[1] = new Vector2Int(73, 0);
				return ring;
			}

			Vector2Int coordinates = new Vector2Int(radius, 0);

			ring[count++] = coordinates;
			ring[count++] = new Vector2Int(-coordinates.x, 0);
			ring[count++] = new Vector2Int(0, coordinates.x);
			ring[count++] = new Vector2Int(0, -coordinates.x);

			while (coordinates.x >= coordinates.y) {
				coordinates.y++;

				// Mid point inside or on the perimeter
				if (auxPoint <= 0) {
					auxPoint = auxPoint + 2 * coordinates.y + 1;
				} else {
					coordinates.x--;
					auxPoint = auxPoint + 2 * coordinates.y - 2 * coordinates.x + 1;
				}

				if (coordinates.x < coordinates.y) {
					break; // Correct early return by breaking the loop
				}

				// Use new instances of Vector2Int for each coordinate
				ring[count++] = new Vector2Int(coordinates.x, coordinates.y);
				ring[count++] = new Vector2Int(-coordinates.x, coordinates.y);
				ring[count++] = new Vector2Int(coordinates.x, -coordinates.y);
				ring[count++] = new Vector2Int(-coordinates.x, -coordinates.y);

				if (coordinates.x != coordinates.y) {
					ring[count++] = new Vector2Int(coordinates.y, coordinates.x);
					ring[count++] = new Vector2Int(-coordinates.y, coordinates.x);
					ring[count++] = new Vector2Int(coordinates.y, -coordinates.x);
					ring[count++] = new Vector2Int(-coordinates.y, -coordinates.x);
				}
			}

			// Add a marker at the end if needed
			if (count < ring.Length) {
				ring[count] = new Vector2Int(73, 0);
			}

			return ring;
		}
		public static Vector2Int[] GetFlatRingMatrix(){
			Vector2Int[][]ringMatrix= new Vector2Int[17][];

			Vector2Int[] ringArray=new Vector2Int[93*17];

			
			for(int i=0; i<17; i++){
				
				ringMatrix[i]=new Vector2Int[93];
				AutomataHelper.GenerateRingCoords(i, ringMatrix[i]);


			}

			for(int n=0;  n<17; n++){
				for(int m=0; m<93; m++){
					ringArray[(n*93)+m]=ringMatrix[n][m];
					Debug.Log($"{ringMatrix[n][m]}, {n*93+m}");
				}

			}

			return ringArray;
       
		}


	} 
	
	
	

    
}