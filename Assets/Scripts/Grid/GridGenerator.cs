using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AutomataUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int radius  = 5; // Grid size

    public GameObject cellPrefab; // Prefab for grid cells
    

    public List<Vector2Int> neighbors;
    
    public int cellCounter;

    public Dictionary<Vector2Int, int> coordinatesToCounter;



    /*Generates a nxn Grid where (0,0) is the center
    in the object exists a list of all the cells its generated.

    */
    // This method will be called from the editor script

    

    public void Start(){
        GenerateGrid();
        
    }



    //Generates n*n grid, has a cell counter andeach time is pressed clears the list of cells chosen
    public void GenerateGrid()
    {
        coordinatesToCounter= new Dictionary<Vector2Int, int>();


        int gridSide=radius*2+1;
        
        int gridSize= gridSide*gridSide;


        cellCounter=0;

        //Delete existing cells || To-do add remove from list
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
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


        // Debug.Log("Grid generated with " + cellCounter + " cells.");

        neighbors.Clear();

    }

    public List<Vector2Int> GetNeighborhood(){
        return neighbors;
        
    }




    //Creates Cell inside grid with name (x,y) 
    public void makeCell(int x, int y,int  counter){
        GameObject cell = Instantiate(cellPrefab, transform);
        cell.name = $"({x},{y})";

        Cell cellScript = cell.GetComponent<Cell>();
        if (cellScript != null)
        {
            cellScript.SetCoordinates(x, y, counter);
            coordinatesToCounter.Add(new Vector2Int(x,y), counter);
            Debug.Log($"Coords: {x},{y} and counter: {counter}");
        }
        
        

        
    }


    
    
    //Fix this horrible monster usea  dictionary instead man
    //Wich basically given a cell finds the symetric cell GameObjects and turns them on or off


    public void AddSymetrically(Cell cell){
        int x= cell.coord.x;
        int y= cell.coord.y;

        //find symetrical pairs 

        Cell[] symetricalPairs= new Cell[4];

        symetricalPairs[0]=cell;

        symetricalPairs[1]= getCellGameObject(new Vector2Int(-x,y));
        symetricalPairs[2]= getCellGameObject(new Vector2Int(x,-y));
        symetricalPairs[3]= getCellGameObject(new Vector2Int(-x,-y));


        foreach(Cell cellScript in symetricalPairs){
            toggleCell(cellScript);
        }
    }

    public void AddRing(Cell cell){

        int x= cell.coord.x;
        int y= cell.coord.y;

        double norm= AutomataHelper.Norm(cell.coord);

        radius= (norm - Math.Floor(norm)>0.5) ? (int)norm+1 : (int)norm;

        List<Vector2Int> coordinatesOfRing= new List<Vector2Int>();
      

        

        Vector2Int coordinates= new Vector2Int(radius, 0);
        int auxPoint= radius -1;


        while(coordinates.x>=0 && coordinates.y<=radius){
            coordinatesOfRing.Add(coordinates);


            coordinates.y++;

            if(AutomataHelper.Norm(new Vector2Int(auxPoint, coordinates.y))>radius){
                coordinates.x--;
                auxPoint--;
            }

            coordinatesOfRing.Add(coordinates);
        }

        foreach(Vector2Int coord in coordinatesOfRing){
            toggleCell(getCellGameObject(coord));
            toggleCell(getCellGameObject(new Vector2Int(-coord.x, coord.y)));
            toggleCell(getCellGameObject(new Vector2Int(coord.x, -coord.y)));
            toggleCell(getCellGameObject(new Vector2Int(-coord.x, -coord.y)));
        }


        



    }
    public Cell getCellGameObject(Vector2Int coord){
        return transform.GetChild(coordinatesToCounter[coord]).GetComponent<Cell>();
    }
    // public void AddSymetrically(Cell cell){
    //     int x = cell.coord.x;
    //     int y = cell.coord.y;
        
    //     toggleCell(cell);


    //     for(int i=0; i<transform.childCount; i++  ){
    //         Transform child=transform.GetChild(i);
    //         Cell cellAux= child.GetComponent<Cell>();

           
    //         if(x!=0 && y!=0){

    //             if(cellAux.coord==new Vector2Int(-x,y)){
    //                 toggleCell(cellAux);   //Changes State
    //             }else if(cellAux.coord==new Vector2Int(x,-y)){
    //                 toggleCell(cellAux);
    //             }else if(cellAux.coord ==new Vector2Int(-x,-y) ){
    //                 toggleCell(cellAux);
    //             }
    //         }else if(x==0){
    //             if(cellAux.coord==new Vector2Int(x,-y)){
    //                 toggleCell(cellAux);
    //             }else if(cellAux.coord==new Vector2Int(y,0)){
    //                 toggleCell(cellAux);
    //             }else if(cellAux.coord==new Vector2Int(-y,0)){
    //                 toggleCell(cellAux);
    //             }
    //         }else{
    //             if(cellAux.coord==new Vector2Int(-x,y)){
    //                 toggleCell(cellAux);
    //             }else if(cellAux.coord==new Vector2Int(0,x)){
    //                 toggleCell(cellAux);
    //             }else if(cellAux.coord==new Vector2Int(0,-x)){
    //                 toggleCell(cellAux);
    //             }
    //         }

            

            

    //     }

    // }

    //Turn cell on or of and add to List
    public void toggleCell(Cell cell){
        if(!cell.state){
            neighbors.Add(cell.coord);
            cell.changeState();
        }
        
    }

    

    //FUNCTION GRAVE YARD -----------------------------------------------------------------------------------------------------------

    //For every Cell child in grid we add it to List<Cells> neighbors
    //Unecessary now cause we only need the ones selected
    // public void fetchListOfCells(){
        
    //     neighbors.Clear();
    //     for(int i=0; i<transform.childCount; i++  ){
    //         Transform child=transform.GetChild(i);
    //         Cell cell= child.GetComponent<Cell>();

            

            
    //         neighbors.Add(cell.coord);

    //     }
            
    // }



    //Atempted to find simetrical cells w a matrix
    // public Cell[] FindSymetricalCells(Cell cell){
    //     Cell[] result= new Cell[4];


    //     int x= cell.coord.x;
    //     int y= cell.coord.y;


    //     if(matrix!=null){
    //         result[0]=cell;
    //         result[1]=matrix.GetValue(-x,y);
    //         result[2]=matrix.GetValue(x,-y);
    //         result[3]=matrix.GetValue(-x,-y);
    //     }else{
    //         Debug.Log("Null matrix");
    //     }
        
        
    // public void adjustCell (int i){

    //     if (transform.childCount > 0)
    //     {
    //         Transform lastChild = transform.GetChild(transform.childCount - 1);
    //         DestroyImmediate(transform);

    //         cellCounter--;
            
    //     }

    // }


    //     return result;
    // }

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
