using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In the context of CodeBlox some levels can have tutorials.
/// This component receives a list of tutorial descriptors, and creates instances of tutorial panels.
/// Also controls which tutorial part to be shown
/// </summary>
public class TutorialLoader : MonoBehaviour
{
    private int currentPage = 1;
    private List<TutorialPanel> tutorialPages = new List<TutorialPanel>();
    [SerializeField] List<TutorialDescriptor> TutorialList;
    [SerializeField] TutorialPanel TemplateTutorialPanel;
    [SerializeField] Button TutorialButton;

    // Start is called before the first frame update
    void Start()
    {        
        // Creates tutorial instances
        foreach (TutorialDescriptor tutorialDescriptor in TutorialList)
        {
            int index = TutorialList.IndexOf(tutorialDescriptor);
            TutorialPanel tutorialPanel = Instantiate(TemplateTutorialPanel, this.transform);
            tutorialPanel.SetText(tutorialDescriptor.ExplanationText);
            tutorialPanel.SetPage(index + 1, TutorialList.Count);
            tutorialPages.Add(tutorialPanel);
            tutorialPanel.gameObject.SetActive(index == 0); // By default just the first page shall be active
        }

        if (tutorialPages.Count == 0)
            TutorialButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextPage()
    {
        TutorialPanel activePage = tutorialPages[currentPage - 1];
        currentPage = currentPage != tutorialPages.Count ? currentPage + 1 : 1;
        TutorialPanel nextPage = tutorialPages[currentPage - 1]; 
        activePage.gameObject.SetActive(false);
        nextPage.gameObject.SetActive(true);
    }

}
