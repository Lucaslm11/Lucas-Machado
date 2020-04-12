using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// This class just saves a level pattern and data on the amount of each blox used
/// A synthesized pattern can be described by sequences of lines. 
/// 
/// Note to self: I thought about marking the end of a line with an L, or U, or S turn, but
///                 the way we are going to pattern stuff, lines just and with an L turn.
///                 That is because, for example, a U turn to be done, the character 
///                 will have to, for example, turn to the left, then walk one step,
///                 and then turn to the left again. So in fact, two lines will be detected.
///                 In a sequence of lines, the last termination shall not be considered, because
///                 it will depend on how the next pattern starts
///                 
/// 
/// 
/// </summary>
public class SynthesizedSolution 
{
    struct Pattern
    {

    }
}

public class LineSequence
{
    public List<Line> seq { get; set; }
    public struct Line
    {
        public int size;
        public int endingType; //-1 for left, 0 for none, 1 for right
    }
}