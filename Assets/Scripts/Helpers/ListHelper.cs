using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListHelper  
{
    /// <summary>
    /// Swaps elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="idxA"></param>
    /// <param name="idxB"></param>
    public static void Swap<T>(List<T> list, int idxA, int idxB)
    {
        T aux = list[idxA];
        list[idxA] = list[idxB];
        list[idxB] = aux;
    }
}
