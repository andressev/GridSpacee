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

    public Sprite spriteOff;

    public Sprite spriteOn;

    private SpriteRenderer render;
    


    public void Start(){
        image= GetComponent<Image>();
        
    }

    public void SetCoordinates(int x, int y, int counter){
        this.coord=new Vector2Int(x,y);

        this.counter=counter;
    }

    

    public void changeState(){
        

        if(this.state){
            image.sprite=spriteOn;
        }else{
            image.sprite=spriteOn;

        }
        state=!state;
        Debug.Log(this.counter);

    }

    


}
