using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehavior : MonoBehaviour
{
  public List<float> sliderValues;
    
    void Start()
    {
        sliderValues= new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform child in transform){
            sliderValues.Add(child.GetComponent<Slider>().value);
        }
    }
}
