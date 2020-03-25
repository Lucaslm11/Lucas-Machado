using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component accepts two game objects, and helps alternating the visibility between 
/// both of them. Only one shall be visibile.
/// </summary>
public class FieldSwitcher : MonoBehaviour
{
    [SerializeField] GameObject Field1;
    [SerializeField] GameObject Field2;

    // Start is called before the first frame update
    void Start()
    {
        if (Field1 != null && Field2 != null)
        {
            SetDefaultStates();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Field1.activeInHierarchy == Field2.activeInHierarchy)
        {
            SetDefaultStates();
        }
    }

    /// <summary>
    /// Alternates visibility states
    /// </summary>
    public void Alternate()
    {
        if (Field1 != null && Field2 != null)
        { 
            Field1.SetActive(!Field1.activeInHierarchy);
            Field2.SetActive(!Field2.activeInHierarchy);
        }
    }

    void SetDefaultStates()
    {
        Field1.SetActive(true);
        Field2.SetActive(false);
    }

    public GameObject GetActiveField()
    {
        if (Field1.activeInHierarchy)
            return Field1; 
        else
            return Field2; 
    }

    public GameObject getField1() { return Field1; }
    public GameObject getField2() { return Field2; }

}
