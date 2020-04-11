using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelHandler;

public class LevelFailurePanel : MonoBehaviour
{
    [SerializeField] Text textBox;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFailure(Evaluation evaluation)
    {
        if (!evaluation.Success)
        {
            textBox.text += LevelCompletionMessages.MAIN_ERROR_TEXT;
            if (evaluation.ObjectiveCheck.mandatoryBloxes)
                textBox.text += LevelCompletionMessages.ERROR_MANDATORY_BLOXES;
            if (evaluation.ObjectiveCheck.mandatorySteps)
                textBox.text += LevelCompletionMessages.ERROR_MANDATORY_STEPS;
            if (evaluation.ObjectiveCheck.wrongSteps > 0)
                textBox.text += string.Format(LevelCompletionMessages.WRONG_STEPS, evaluation.ObjectiveCheck.wrongSteps);


        }
        else
            textBox.text = null;
    }

    public void SetDisplayState(bool display=false)
    {
        gameObject.SetActive(display);
    }


}
