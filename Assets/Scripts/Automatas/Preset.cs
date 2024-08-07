using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preset : MonoBehaviour
{

    public GameObject simulation;
    private MNCA mnca;
    public string sequence;
    // Start is called before the first frame update
    void Start()
    {
        mnca=simulation.GetComponent<MNCA>();
    }

    // Update is called once per frame
    public void setSequence(){
        mnca.SetAutomaton(sequence);
    }
}
