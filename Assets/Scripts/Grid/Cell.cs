using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Vector2Int coord;
    public bool state=false;
    public int counter; 
    private Image image;
    


    public void Start(){
        image= GetComponent<Image>();
    }

    public void SetCoordinates(int x, int y, int counter){
        this.coord=new Vector2Int(x,y);

        this.counter=counter;
    }

    

    public void changeState(){

        if(this.state){
            image.color=new Color(0.33f,.33f,0.33f,1);
        }else{
            image.color=new Color(0.8f,.8f,0.8f,1);

        }
        state=!state;
        Debug.Log(this.counter);

    }

    


}
