using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    private const string PAGINATION_FORMAT = "{0}/{1}";
    [SerializeField] Text ExplanationText;
    [SerializeField] Text PaginationText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        ExplanationText.text = text;
    }

    public void SetPage(int page, int totalPages)
    {
        PaginationText.text = string.Format(PAGINATION_FORMAT, page, totalPages);
    }
}
