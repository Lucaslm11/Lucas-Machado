using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static GameObjectHelper;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SquareTile TemplateTile; // all the others will be constructed from this one
    [SerializeField] Text MandatoryObjectivesText;
    [SerializeField] Text OptionalObjectivesText;
    List<SquareTile> squareTiles = new List<SquareTile>();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void LoadSquareTiles(LevelDescriptor levelConfiguration)
    {
        uint width = levelConfiguration.PlotWidth;
        uint height = levelConfiguration.PlotLength;

        List<ObjectiveStep> mandatorySteps = levelConfiguration.MandatorySteps;

        BoundingBox2D tileBBox = GameObjectHelper.getBoundingBoxInWorld(TemplateTile.gameObject);
        float sqWidth = GameObjectHelper.getWidthFromBBox(tileBBox);
        float sqHeight = GameObjectHelper.getWidthFromBBox(tileBBox);

        float sqDistance = sqWidth / 10; // distance between tiles

        for (int plotY = 0; plotY < height; plotY++) //iterates through lines (y)
        {
            for (int plotX = 0; plotX < width; plotX++) //iterates through columns (x)
            {
                // Checks if this is a mandatory step, and obtains info
                ObjectiveStep step = levelConfiguration.MandatorySteps.Where(s => s.CoordinateInPlot.x == plotX && s.CoordinateInPlot.y == plotY).FirstOrDefault();

                // Creates the tile
                SquareTile tile = (plotX + plotY) == 0 ? TemplateTile : TemplateTile.CloneThisTile();
                //tile.transform.parent = TemplateTile.gameObject.transform.parent;
                //tile.transform.position = TemplateTile.transform.position; 

                // Sets the tile colour
                tile.SetColour(step == null ? SquareTile.STExpectedState.NOT_TO_STEP :
                                    (step.SpecialAction ? SquareTile.STExpectedState.TO_EXECUTE_SPECIAL_ACTION : SquareTile.STExpectedState.TO_STEP));

                // Sets the coordinates text display
                tile.SetCoordinateText(plotX, plotY);

                // Creates a new position
                Vector3 newTilePos = tile.transform.position;
                newTilePos.x += plotX * (sqWidth + sqDistance);
                newTilePos.y += plotY * (sqHeight + sqDistance);

                // Sets the new position
                tile.transform.position = newTilePos;

                squareTiles.Add(tile);
            }
        }

    }


    public void LoadTexts(LevelDescriptor levelConfiguration)
    {

        List<string> mandTexts = new List<string>();
        if (levelConfiguration.MandatoryBloxes.Count > 0)
            mandTexts.Add("-" + string.Format(ObjectivePanelMessages.MANDATORY_BLOXES,
                string.Join(", ", levelConfiguration.MandatoryBloxes.Select(m => string.Format(ObjectivePanelMessages.EXPECTED_BLOX_TEMPLATE, m.MinimumQuantity, m.Blox.gameObject.name.Replace("Blox", ""))))));
        mandTexts.Add("-" + ObjectivePanelMessages.MANDATORY_STEPS);
        if (levelConfiguration.SpecialActionBlox != null)
            mandTexts.Add("-" + string.Format(ObjectivePanelMessages.MANDATORY__SPECIAL_STEPS, levelConfiguration.SpecialActionBlox.ACTION_DESCRIPTION));

        //ADICIONAR ENTRADA PARA SPECIAL BLOX NO DESCRIPTOR E ADICIONA-LO AOS OBRIGATORIOS

        MandatoryObjectivesText.text = string.Join("\n", mandTexts);

        List<string> optionalTexts = new List<string>();
        if (levelConfiguration.OptionalExpectedBloxes.Count > 0)
            optionalTexts.Add("-" + string.Format(ObjectivePanelMessages.OPTIONAL_BLOXES,
            string.Join(", ", levelConfiguration.OptionalExpectedBloxes.Select(m => string.Format(ObjectivePanelMessages.EXPECTED_BLOX_TEMPLATE, m.MinimumQuantity, m.Blox.gameObject.name.Replace("Blox", ""))))));

        optionalTexts.Add("-" + string.Format(ObjectivePanelMessages.OPTIONAL_ATTEMPTS, levelConfiguration.MaxAttempts));
        optionalTexts.Add("-" + string.Format(ObjectivePanelMessages.OPTIONAL_LINES, levelConfiguration.MaxCodeLinesExpected));
        optionalTexts.Add("-" + string.Format(ObjectivePanelMessages.TIME, levelConfiguration.MaxTimeInMinutes));

        OptionalObjectivesText.text = string.Join("\n", optionalTexts);

    }
}
