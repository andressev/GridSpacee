using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int radius  = 5; // Grid size

    public GameObject cellPrefab; // Prefab for grid cells
    public Transform gridParent; // Parent object for grid cells (Grid Layout Group)

    public List<Cell> neighbors;
    
    public int cellCounter;

    public NxNMatrix matrix;

    /*Generates a nxn Grid where (0,0) is the center
    in the object exists a list of all the cells its generated.

    */
    // This method will be called from the editor script
    public void Start(){
        GenerateGrid();
    }
    public void GenerateGrid()
    {


        int gridSide=radius*2+1;
        
        int gridSize= gridSide*gridSide;


        cellCounter=0;

        //Delete existing cells || To-do add remove from list
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }

       




        //Gen grid of Cells
        for (int y = -radius; y <= radius; y++  )
        {
            for (int x = -radius; x <= radius; x++)
            {
                makeCell(x,y,cellCounter);
                cellCounter++;
            }
        }

        //fillListOfCells


        Debug.Log("Grid generated with " + cellCounter + " cells.");

        neighbors.Clear();

    }



    // public void adjustCell (int i){

    //     if (gridParent.childCount > 0)
    //     {
    //         Transform lastChild = gridParent.GetChild(gridParent.childCount - 1);
    //         DestroyImmediate(gridParent);

    //         cellCounter--;
            
    //     }

    // }

    //Creates Cell inside grid with name (x,y) 
    public void makeCell(int x, int y,int  counter){
        GameObject cell = Instantiate(cellPrefab, gridParent);
        cell.name = $"({x},{y})";

        Cell cellScript = cell.GetComponent<Cell>();
        if (cellScript != null)
        {
            cellScript.SetCoordinates(x, y, counter);
        }
        

        
    }


    //For every Cell child in grid we add it to List<Cells> neighbors
    public void fetchListOfCells(){
        
        neighbors.Clear();
        for(int i=0; i<gridParent.childCount; i++  ){
            Transform child=gridParent.GetChild(i);
            Cell cell= child.GetComponent<Cell>();

            

            
            neighbors.Add(cell);

        }
            
    }
    
    
    //Fix this horrible monster
    public void AddSymetrically(Cell cell){
        int x = cell.coord.x;
        int y = cell.coord.y;
        
        toggleCell(cell);


        for(int i=0; i<gridParent.childCount; i++  ){
            Transform child=gridParent.GetChild(i);
            Cell cellAux= child.GetComponent<Cell>();

           
            if(x!=0 && y!=0){

                if(cellAux.coord==new Vector2Int(-x,y)){
                    toggleCell(cellAux);   //Changes State
                }else if(cellAux.coord==new Vector2Int(x,-y)){
                    toggleCell(cellAux);
                }else if(cellAux.coord ==new Vector2Int(-x,-y) ){
                    toggleCell(cellAux);
                }
            }else if(x==0){
                if(cellAux.coord==new Vector2Int(x,-y)){
                    toggleCell(cellAux);
                }else if(cellAux.coord==new Vector2Int(y,0)){
                    toggleCell(cellAux);
                }else if(cellAux.coord==new Vector2Int(-y,0)){
                    toggleCell(cellAux);
                }
            }else{
                if(cellAux.coord==new Vector2Int(-x,y)){
                    toggleCell(cellAux);
                }else if(cellAux.coord==new Vector2Int(0,x)){
                    toggleCell(cellAux);
                }else if(cellAux.coord==new Vector2Int(0,-x)){
                    toggleCell(cellAux);
                }
            }

            

            

        }

    }
    public void toggleCell(Cell cell){
        if(!cell.state){
            neighbors.Add(cell);
            cell.changeState();
        }
        
    }






    public Cell[] FindSymetricalCells(Cell cell){
        Cell[] result= new Cell[4];


        int x= cell.coord.x;
        int y= cell.coord.y;


        if(matrix!=null){
            result[0]=cell;
            result[1]=matrix.GetValue(-x,y);
            result[2]=matrix.GetValue(x,-y);
            result[3]=matrix.GetValue(-x,-y);
        }else{
            Debug.Log("Null matrix");
        }
        
        


        return result;
    }

    /*Given a cell and using FindSymetricalCells we receieve an array of four cells, containing the cell itself
    then we change the state of each cell
    
    */
    // public void AddSymetrically(Cell cell){
    //     Vector2Int coord=cell.coord;

    //     Cell[] cellList= FindSymetricalCells(cell);

    //     for(int i=0; i<4; i++){
           
    //         cellList[i].changeState();
    //     }

    //Gets the index through the cell coordinates just getting all four cuadrants
    // }


    // public Cell FindSymetricalCells(Cell cell){
    // }

    
    // public int GetCellIndex(Vector2Int coord){
        

    //     Vector2Int result= new Vector2Int(coord.x+radius, coord.y+radius); //Displace x 
        
    //     int value= (result.x+result.y*radius)-1;
    //     Debug.Log("The index is "+ value);

    //     return value;
        
    // }


}
