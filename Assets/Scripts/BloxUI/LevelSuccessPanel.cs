using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelHandler;

public class LevelSuccessPanel : MonoBehaviour
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

    public void SetSuccess(Evaluation evaluation)
    {
        if (evaluation.Success)
        {
            textBox.text += LevelCompletionMessages.MAIN_SUCCESS_TEXT;
            textBox.text += LevelCompletionMessages.MAIN_OBJECTIVES;
            textBox.text += string.Format(LevelCompletionMessages.OPTIONAL_BLOXES,evaluation.ObjectiveCheck.numberOfOptionalBloxesUsed, evaluation.ObjectiveCheck.maxOptionalBloxesExpected, Evaluation.SCORE_PER_STAR - evaluation.optionalBloxDiscount);
            textBox.text += string.Format(LevelCompletionMessages.TIME,Math.Max(Math.Round(evaluation.ObjectiveCheck.exceededMinutes,2), 0 ), Evaluation.SCORE_PER_STAR - evaluation.timeDiscount);
            textBox.text += string.Format(LevelCompletionMessages.ATTEMPTS,evaluation.ObjectiveCheck.exceededAttempts, evaluation.ObjectiveCheck.maxAttemptsExpected, Evaluation.SCORE_PER_STAR - evaluation.attemptsDiscount);
            textBox.text += string.Format(LevelCompletionMessages.LINES,evaluation.ObjectiveCheck.exceededLines, evaluation.ObjectiveCheck.maxLinesExpected, Evaluation.SCORE_PER_STAR - evaluation.linesDiscount);
            textBox.text += string.Format(LevelCompletionMessages.SUM,evaluation.Score);
            textBox.text += string.Format(LevelCompletionMessages.STARS,evaluation.Stars);

            //if (evaluation.ObjectiveCheck.mandatoryBloxes)
            //    textBox.text += LevelCompletionMessages.ERROR_MANDATORY_BLOXES;
            //if (evaluation.ObjectiveCheck.mandatorySteps)
            //    textBox.text += LevelCompletionMessages.ERROR_MANDATORY_STEPS;
            //if (evaluation.ObjectiveCheck.wrongSteps > 0)
            //    textBox.text += string.Format(LevelCompletionMessages.WRONG_STEPS, evaluation.ObjectiveCheck.wrongSteps);


        }
        else
            textBox.text = null;
    }

    public void SetDisplayState(bool display = false)
    {
        gameObject.SetActive(display);
    }


}
