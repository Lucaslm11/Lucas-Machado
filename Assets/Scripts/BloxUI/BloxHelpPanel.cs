using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloxHelpPanel : MonoBehaviour
{
    [SerializeField] Text HelpText;
    [SerializeField] RawImage Image;

    ABlox LastClickedBlox = null;
    // Start is called before the first frame update
    void Start()
    {

        Image.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (ABlox.LastClickedBlox != LastClickedBlox)
        {
            HelpText.text = ABlox.LastClickedBlox.HelpText;
            if (ABlox.LastClickedBlox.HelpExampleTexture != null)
            {
                Image.texture = ABlox.LastClickedBlox.HelpExampleTexture;
                Image.gameObject.SetActive(true);
            }
            else
            {
                Image.gameObject.SetActive(false);
            }
            LastClickedBlox = ABlox.LastClickedBlox;
        }
         
    }

}
