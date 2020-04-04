using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The SquareTile represents a the representation of a plot tile in the ObjectivePanel
/// </summary>
public class SquareTile : MonoBehaviour
{
    // Each tile will have a different colour if it is expected to be stepped or not,
    // or to have a special action executed or not
    public enum STExpectedState
    {
        NOT_TO_STEP,
        TO_STEP,
        TO_EXECUTE_SPECIAL_ACTION
    }

    [SerializeField] Text XCoordinateText;
    [SerializeField] Text YCoordinateText;
    [SerializeField] Color NotToStepColor = Color.white;
    [SerializeField] Color ToStepColor = Color.blue;
    [SerializeField] Color ToExecuteSpecialActionColor = Color.green;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColour(STExpectedState expectedState)
    {
        Image img = this.GetComponent<Image>();
        switch (expectedState)
        {
            case STExpectedState.NOT_TO_STEP:
                img.color = NotToStepColor;
                break;
            case STExpectedState.TO_STEP:
                img.color = ToStepColor;
                break;
            case STExpectedState.TO_EXECUTE_SPECIAL_ACTION:
                img.color = ToExecuteSpecialActionColor;
                break;
        }
    }

    public void SetCoordinateText(int x, int y)
    {
        XCoordinateText.text = x.ToString();
        YCoordinateText.text = y.ToString();

        bool dontDestroyX = false;
        bool dontDestroyY = false;

        if (x == 0)
        {
            dontDestroyY = true;
        }
        if (y == 0)
        {
            dontDestroyX = true;
        }

        if (!dontDestroyY)
            Destroy(YCoordinateText);
        if (!dontDestroyX)
            Destroy(XCoordinateText);

    }


    public SquareTile CloneThisTile()
    {
        return Instantiate(this, transform.parent);
    }

}
