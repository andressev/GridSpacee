using System.Collections;
using System.Collections.Generic;
using AutomataUtilities;
using UnityEngine;

public class NeighborhoodTemplate : MonoBehaviour
{
    // Start is called before the first frame update

    //Checks if two neighborhoods have already been picked if not add itself to the picked list

    public Neighborhood nh;

    public void OnClick(){
        Transform pickedNeighborhoods=GetComponentInParent<SavedNeighboorhods>().picked;

        
        //Pulls data, instantiates the sets data
        if(pickedNeighborhoods.childCount<2){

            Neighborhood nh= GetComponent<NeighborhoodTemplate>().nh;
            var copyNeighborhood= Instantiate(gameObject, pickedNeighborhoods);

            NeighborhoodTemplate template = copyNeighborhood.GetComponent<NeighborhoodTemplate>();

            template.nh=nh;
            

        }
    }
}
