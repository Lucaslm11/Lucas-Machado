using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorPopup : MonoBehaviour
{
    private List<BloxValidationError> ErrorList;
    [SerializeField] ErrorMessage ErrorMessageTemplate;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Loads errors into popup
    /// </summary>
    /// <param name="errorList"></param>
    public void LoadErrors(List<BloxValidationError> errorList)
    {
        ErrorList = errorList;
        RectTransform templateRectTrf = ErrorMessageTemplate.GetComponent<RectTransform>();
        float errorMessageVerticalSpacing = templateRectTrf.rect.height / 4;

        int i = 0;
        foreach(BloxValidationError error in errorList)
        {
            float templateY = templateRectTrf.localPosition.y;
            float newY = templateY + i * (templateRectTrf.rect.height + errorMessageVerticalSpacing);
            ErrorMessage newMessage = Instantiate(ErrorMessageTemplate, templateRectTrf.parent);
            newMessage.SetErrorMessage(error);
            i++;
        }
    }
}
