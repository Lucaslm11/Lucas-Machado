﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IfBlox : Blox
{
    // Saves a game object to next side to side with this one
    GameObject objectToNest;
   
   
    // Start is called before the first frame update
    void Start()
    {
        print("HIIII");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void OnEndDrag(PointerEventData eventData)
    {

        // ESTE PEDAÇO DE CÓDIGO NÃO FUNCIONA ATUALMENTE, PORQUE O QUE SE ESTÁ A ARRASTAR É O INT E NÃO O IF. SÓ QUANDO O IF É ARRASTADO
        base.OnEndDrag(eventData);

    }

    public override bool ValidateNesting(GameObject objectToNest)
    {
        // For now accepts, but must validate if object is nestable
        return true;
    }
}
