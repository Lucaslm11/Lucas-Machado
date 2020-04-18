using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabDescriptor
    {
        [SerializeField] public TabButton TabButton;
        [SerializeField] public RectTransform Panel;

        public bool Active { get; set; }
    }

    [SerializeField] List<TabDescriptor> TabDescriptors;

    // Start is called before the first frame update
    void Start()
    {
        if(TabDescriptors!=null && TabDescriptors.Count  > 0)
        {
            TabDescriptors[0].Active = true;

            foreach (TabDescriptor descriptor in TabDescriptors)
            {
                descriptor.TabButton.SetListener(delegate { ManageClick(descriptor.TabButton); });
            }

        }
         
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ManageClick(TabButton btn)
    {
        TabDescriptor activeTab = TabDescriptors.First(t => t.Active);
        TabDescriptor currentTab = TabDescriptors.First(t => t.TabButton == btn);
        InactivateTab(activeTab);
        ActivateTab(currentTab);
    }


    void ActivateTab(TabDescriptor tab)
    {
        tab.Active = true;
        tab.Panel.gameObject.SetActive(true);
    }

    void InactivateTab(TabDescriptor tab)
    {
        tab.Active = false;
        tab.Panel.gameObject.SetActive(false);
    }
}



