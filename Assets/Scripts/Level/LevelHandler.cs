using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static LevelDescriptor;

/// <summary>
/// This component will be essencial for the loading of configuration of a level, and evaluation of the level completion
/// Each level will have 5 objectives:
///     1 - Complete in time
///     2 - Use a number of attempts lesser than the maximum
///     3 - Use a number of blocks lesser than the maximum
///     3 - Use the expected bloxes
///     4 - Complete the in game objective
/// </summary>
public class LevelHandler : MonoBehaviour
{
    [SerializeField] LevelDescriptor LevelConfiguration;

    public struct ObjectiveCheck
    {
        public bool mandatorySteps;   // Did the correct steps
        public bool mandatoryBloxes;  // Used the mandatory bloxes

        public int wrongSteps;
        public double exceededMinutes;         // Extra time used. 
        public int exceededAttempts;     // Attempts not exceeded
        public int exceededLines;        // Used maximum lines

        public float optionalBloxesUsedPercentage;    // Used the optional bloxes
    }

    public class Evaluation
    {

        const int MAX_SCORE = 5;
        public int Score { get; }
        public int Stars { get; set; }
        public bool Success { get; }
        public ObjectiveCheck ObjectiveCheck { get; }
        public Evaluation(ObjectiveCheck objectiveCheck)
        {
            this.ObjectiveCheck = objectiveCheck;
            Success = objectiveCheck.mandatorySteps && objectiveCheck.mandatoryBloxes;

            double discount = 0;
            discount -= CalcDiscount(ObjectiveCheck.exceededMinutes);
            discount -= CalcDiscount(ObjectiveCheck.exceededAttempts);
            discount -= CalcDiscount(ObjectiveCheck.exceededLines);
            discount -= 1-ObjectiveCheck.optionalBloxesUsedPercentage;
            
            double score = MAX_SCORE + discount;

            Score = Convert.ToInt32(score * 1000);
            Stars = Convert.ToInt32(Math.Floor(score));

        }

        // Computes (1-1/(x+1)) in the domain of [0,+inf[
        // If x < 0 returns 0
        private double CalcDiscount(double x)
        {
            return x < 0 || double.IsNaN(x) ? 0 : 1-1/(x+1);
        }

    }

    int numberOfAttempts = 0;
    private DateTime startTime;
    private List<PlotTile> tilesInScene = new List<PlotTile>();
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;
        BuildTilePlot();
        SetCharacter();
        numberOfAttempts = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Tile plot handling
    // Builds the tile plot
    public void BuildTilePlot()
    {
        PlotTile templateTile = LevelConfiguration.FirstTile;
        Renderer templateTileRenderer = templateTile.GetComponent<Renderer>();
        // The extents give half of each dimension (width, length, and height)
        Vector3 tileExtents = templateTileRenderer.bounds.extents;
        float tileWidth = 2 * tileExtents.x;
        float tileLength = 2 * tileExtents.z;
        float tileHeight = 2 * tileExtents.y;

        // Important note!
        // The coordinates of a Tile in the Plot Tile do not translate directly to the Coordinates in world
        // plotX <-> worldX, plotY <-> worldZ, plotZ <-> worldY
        // This is to facilitate the configuration of the level, turning it more readable

        //tilesInScene.Add(tile);
        for (int plotY = 0; plotY < LevelConfiguration.PlotHeight; plotY++) //iterates through lines (y)
        {
            for (int plotX = 0; plotX < LevelConfiguration.PlotWidth; plotX++) //iterates through columns (x)
            {
                int plotZ = 0;
                //Verifies for this (plotX,plotY) if there is a special coordinate that modifies heigth
                Vector3Int specialCoordinate = LevelConfiguration.SpecialCoordinates.Where(a => a.x == plotX && a.y == plotY).FirstOrDefault();
                if (specialCoordinate != null)
                {
                    plotZ = specialCoordinate.z;
                }

                // Gets a new tile (or reuses the template if we are on the first position)
                PlotTile newTile = plotY == 0 && plotX == 0 ? templateTile : Instantiate(templateTile);
                newTile.PlotPosition = new Vector3Int(plotX, plotY, plotZ);

                //Defines the new tile position in the world
                Vector3 newTilePosition = newTile.transform.position;
                newTilePosition.x += plotX * tileWidth;
                newTilePosition.y += plotZ * tileHeight;
                newTilePosition.z += plotY * tileLength;

                //Sets the new position
                newTile.transform.position = newTilePosition;

                //Saves the tile in the tile list
                tilesInScene.Add(newTile);
            }
        }
    }

    public void ResetTilePlotState()
    {
        if (tilesInScene != null)
            tilesInScene.ForEach(tile => { tile.Reset(); });
    }
    #endregion

    public Evaluation EvaluateLevel(RootBlox rootBlox)
    {
        ObjectiveCheck check;
        int stars = 0;
        /// Each level will have 5 objectives:
        ///     1 - Complete in time
        ///     2 - Use a number of attempts lesser than the maximum
        ///     3 - Use a number of blocks lesser than the maximum
        ///     4 - Use the expected optional bloxes 
        ///     5 - Complete the in game objective (mandatory steps, bloxes, special actions)
        ///     

        // 1. Time evaluation
        DateTime expectedEndTime = startTime.AddMinutes(LevelConfiguration.MaxTimeInMinutes);
        check.exceededMinutes = (DateTime.Now - expectedEndTime).TotalMinutes;
        
        // 2. Number of attempts
        check.exceededAttempts = numberOfAttempts - (int)LevelConfiguration.MaxAttempts;

        // 3. Use a number of code lines lesser than the maximum
        check.exceededLines = rootBlox.GetChildBloxListInVerticalOrder().Count - (int)LevelConfiguration.MaxCodeLinesExpected; 

        // 4. Right steps. This is a mandatory object, so level shall fail when not accomplished
        // 4.1 Get all the mandatory steps that were executed, with the special action included
        // The final count should match the count of LevelConfiguration.MandatorySteps
        var queryMandatoryStepsCompleted = from mandatoryStep in LevelConfiguration.MandatorySteps
                                           join plotTile in tilesInScene on mandatoryStep.CoordinateInPlot equals plotTile.PlotPosition
                                           where mandatoryStep != null && plotTile != null && plotTile.SpecialActionExecuted == mandatoryStep.SpecialAction && plotTile.Stepped
                                           select plotTile;


        bool allTheMandatoryStepsDone = queryMandatoryStepsCompleted.Count() == LevelConfiguration.MandatorySteps.Count;

        // 4.2 Get all the the not expected steps. It is mandatory that the user restricts to the mandatory steps
        var queryNotExpectedSteps = from plotTile in tilesInScene
                                    join mandatoryStep in LevelConfiguration.MandatorySteps on plotTile.PlotPosition equals mandatoryStep.CoordinateInPlot into mSteps
                                    from step in mSteps.DefaultIfEmpty()
                                    where step == null && plotTile.Stepped
                                    select plotTile;

        bool notExpectedStepsExist = queryNotExpectedSteps.Count() > 0;

        check.wrongSteps = queryNotExpectedSteps.Count();
        check.mandatorySteps = !notExpectedStepsExist && allTheMandatoryStepsDone;


        // 5. This list contains the distinct blox types, and their count
        List<Tuple<Type, int>> usedBloxTypeCount = rootBlox.GetAllBloxesBellow().GroupBy(b => b.GetType()).Select(g => new Tuple<Type, int>(g.First().GetType(), g.Count())).ToList();

        List<ExpectedBlox> mandatoryBloxesUsed = LevelConfiguration.MandatoryBloxes.Where(mb => usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType && b.Item2 >= mb.MinimumQuantity)).ToList();

        check.mandatoryBloxes = mandatoryBloxesUsed.Count == LevelConfiguration.MandatoryBloxes.Count();

        
        List<ExpectedBlox> expectedOptionalBloxesUsed = LevelConfiguration.ExpectedBloxes.Where(mb => usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType && b.Item2 >= mb.MinimumQuantity)).ToList();

        check.optionalBloxesUsedPercentage = LevelConfiguration.ExpectedBloxes.Count == 0 ? 1 : expectedOptionalBloxesUsed.Count * 1f / LevelConfiguration.ExpectedBloxes.Count;



        return new Evaluation(check);
    }

    public void incrementAttempts()
    {
        numberOfAttempts++;
    }



    #region Character handling
    public void SetCharacter()
    {
        CubotController cubot = LevelConfiguration.Character;
        cubot.SetUp(LevelConfiguration, tilesInScene);
    }


    #endregion


}

