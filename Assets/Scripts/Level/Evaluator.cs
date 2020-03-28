using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelDescriptor;

/// <summary>
/// This component will be essencial for the configuration of, and evaluation of the level completion
/// Each level will have 5 objectives:
///     1 - Complete in time
///     2 - Use a number of attempts lesser than the maximum
///     3 - Use a number of blocks lesser than the maximum
///     3 - Use the expected bloxes
///     4 - Complete the in game objective
/// </summary>
public class Evaluator : MonoBehaviour
{
    private DateTime startTime;

    [SerializeField] LevelDescriptor LevelConfiguration;


    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
