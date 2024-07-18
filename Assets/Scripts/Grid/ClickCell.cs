using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class ClickCell : MonoBehaviour
{
    // Start is called before the first frame update
    private Image image;
    private Cell cell;

    private GridGenerator grid;
    public void Start()
    {
        image= GetComponent<Image>();
        cell= GetComponent<Cell>();
        grid= GetComponentInParent<GridGenerator>();
        

    }
    public void OnImageClick()
    {
        
        if (grid != null && cell != null)
        {
            grid.AddSymetrically(cell);
            Debug.Log("Button clicked and AddSymetrically called.");
        }
        else
        {
            Debug.LogError("Grid or cell reference is missing.");
        }
              
        Debug.Log("working button");
    } 
}
