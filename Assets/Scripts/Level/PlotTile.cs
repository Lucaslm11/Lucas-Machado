using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotTile : MonoBehaviour
{
    /// <summary>
    /// Saves the position of this tile in the TilePlot
    /// Note:
    /// x - position along the width (corresponds to the X axis in world)
    /// y - position along the length (corresponds to the Z axis in the world)
    /// z - height displacement (corresponds to the Y axis in the world)
    /// </summary>
    public Vector3Int PlotPosition { get; set; }
    public bool Stepped { get; set; }
    public bool SpecialActionExecuted { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step(bool executeSpecialAction=false)
    {
        Stepped = true;
        SpecialActionExecuted = executeSpecialAction;
    }

    public void Reset()
    {
        Stepped = false;
        SpecialActionExecuted = false;
    }
}
