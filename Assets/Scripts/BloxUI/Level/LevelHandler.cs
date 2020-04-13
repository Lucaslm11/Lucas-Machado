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
    [SerializeField] ObjectivePanel ObjectivePanel;
    [SerializeField] Timer Clock;

    public struct ObjectiveCheck
    {
        public bool mandatorySteps;   // Did the correct steps
        public bool mandatoryBloxes;  // Used the mandatory bloxes

        public int wrongSteps;
        public double exceededMinutes;         // Extra time used. 
        public double maxMinutesExpected;          
        public int exceededAttempts;     // Attempts not exceeded
        public int numberOfAttempts;     // Attempts not exceeded
        public int maxAttemptsExpected;     // Attempts not exceeded
        public int exceededLines;        // Used maximum lines
        public int usedLines;        // Used maximum lines
        public int maxLinesExpected;        // Used maximum lines

        public float optionalBloxesUsedPercentage;    // Used the optional bloxes
        public float numberOfOptionalBloxesUsed;    // Used the optional bloxes
        public float maxOptionalBloxesExpected;    // Used the optional bloxes
    }

    public class Evaluation
    {

        public const int MAX_SCORE = 5000;
        public const int SCORE_PER_STAR = 1000;
        public int Score { get; }
        public int Stars { get; set; }
        public bool Success { get; }
        public ObjectiveCheck ObjectiveCheck { get; }

        public int timeDiscount { get; }
        public int optionalBloxDiscount { get; }
        public int linesDiscount { get; }
        public int attemptsDiscount { get; }

        public Evaluation(ObjectiveCheck objectiveCheck)
        {
            this.ObjectiveCheck = objectiveCheck;
            Success = objectiveCheck.mandatorySteps && objectiveCheck.mandatoryBloxes;

            //double discount = 0;

            timeDiscount = Convert.ToInt32(Math.Floor(CalcDiscount(ObjectiveCheck.exceededMinutes)* SCORE_PER_STAR));
            linesDiscount = Convert.ToInt32(Math.Floor(CalcDiscount(ObjectiveCheck.exceededLines) * SCORE_PER_STAR));
            attemptsDiscount = Convert.ToInt32(Math.Floor(CalcDiscount(ObjectiveCheck.exceededAttempts) * SCORE_PER_STAR));
            optionalBloxDiscount = Convert.ToInt32(Math.Floor((1 - ObjectiveCheck.optionalBloxesUsedPercentage) * SCORE_PER_STAR));

            Score = MAX_SCORE - timeDiscount - linesDiscount - attemptsDiscount - optionalBloxDiscount;
            Stars = Convert.ToInt32(Math.Floor(Score*1.0f/ SCORE_PER_STAR));
            //discount -= CalcDiscount(ObjectiveCheck.exceededMinutes);
            //discount -= CalcDiscount(ObjectiveCheck.exceededAttempts);
            //discount -= CalcDiscount(ObjectiveCheck.exceededLines);
            //discount -= 1-ObjectiveCheck.optionalBloxesUsedPercentage;
            
            //double score = MAX_SCORE + discount;

            //Score = Convert.ToInt32(score * 1000);
            //Stars = Convert.ToInt32(Math.Floor(score));

        }

        // Computes (1-1/(x+1)) in the domain of [0,+inf[
        // If x < 0 returns 0
        public double CalcDiscount(double x)
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
        LevelConfiguration.Configure();
        startTime = DateTime.Now;
        BuildTilePlot();
        this.ObjectivePanel.LoadSquareTiles(LevelConfiguration);
        this.ObjectivePanel.LoadTexts(LevelConfiguration);
        SetCharacter();
        numberOfAttempts = 0;

        Clock.LoadTimer(LevelConfiguration.MaxTimeInMinutes);
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
        for (int plotY = 0; plotY < LevelConfiguration.PlotLength; plotY++) //iterates through lines (y)
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


    #region Hints and evaluation
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
        check.numberOfAttempts = numberOfAttempts;
        // 3. Use a number of code lines lesser than the maximum
        check.exceededLines = rootBlox.GetChildBloxListInVerticalOrder().Count - (int)LevelConfiguration.MaxCodeLinesExpected;
        check.usedLines = rootBlox.GetChildBloxListInVerticalOrder().Count;

        // 4. Right steps. This is a mandatory object, so level shall fail when not accomplished
        // 4.1 Get all the mandatory steps that were executed, with the special action included
        // The final count should match the count of LevelConfiguration.MandatorySteps
        //var queryMandatoryStepsCompleted = from mandatoryStep in LevelConfiguration.MandatorySteps
        //                                   join plotTile in tilesInScene on mandatoryStep.CoordinateInPlot equals plotTile.PlotPosition
        //                                   where mandatoryStep != null && plotTile != null && plotTile.SpecialActionExecuted == mandatoryStep.SpecialAction && plotTile.Stepped
        //                                   select plotTile;

        var queryMandatoryStepsCompleted  = LevelConfiguration.MandatorySteps.Where(ms => tilesInScene.Exists(t=>t.Stepped && t.SpecialActionExecuted == ms.SpecialAction && t.PlotPosition == ms.CoordinateInPlot));

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

        List<ExpectedBlox> mandatoryBloxesFullyUsed = LevelConfiguration.MandatoryBloxes.Where(mb => usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType && b.Item2 >= mb.MinimumQuantity)).ToList();

        check.mandatoryBloxes = mandatoryBloxesFullyUsed.Count == LevelConfiguration.MandatoryBloxes.Count();

        
        List<ExpectedBlox> expectedOptionalBloxesUsed = LevelConfiguration.OptionalExpectedBloxes.Where(mb => usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType)).ToList();
        int numberOfOptionalBloxesUsed = usedBloxTypeCount
                                            // Gets all the used bloxes that are of the type of one expected to be used
                                            .Where(b => LevelConfiguration.OptionalExpectedBloxes.Exists(mb => mb.BloxType == b.Item1))
                                            // Gets the amount used. If the amount used is bigger than the minimum expected, uses the later value
                                            // If it is expected to use 2 bloxes of Int, and 1 of If, we don't want to classify a full use of option bloxes
                                            // if the user put 3 Int bloxes
                                            .Select(b => Math.Min(b.Item2, LevelConfiguration.OptionalExpectedBloxes.First(o => o.BloxType == b.Item1).MinimumQuantity)).Sum();
        

        check.optionalBloxesUsedPercentage = LevelConfiguration.OptionalExpectedBloxes.Count == 0 ? 1 : numberOfOptionalBloxesUsed * 1f / LevelConfiguration.OptionalExpectedBloxes.Select(o => o.MinimumQuantity).Sum();

        check.maxAttemptsExpected = Convert.ToInt32(LevelConfiguration.MaxAttempts);
        check.maxLinesExpected = Convert.ToInt32(LevelConfiguration.MaxCodeLinesExpected);
        check.maxMinutesExpected = Convert.ToInt32(LevelConfiguration.MaxTimeInMinutes);
        check.maxOptionalBloxesExpected = Convert.ToInt32(LevelConfiguration.OptionalExpectedBloxes.Select(o=>o.MinimumQuantity).Sum());
        check.numberOfOptionalBloxesUsed = numberOfOptionalBloxesUsed;
        return new Evaluation(check);
    }

    public void incrementAttempts()
    {
        numberOfAttempts++;
    }


    /// <summary>
    /// What Hint looks for:
    /// Gets the placed bloxes
    /// looks at the bloxes that are expected to be placed
    /// 
    /// looks for the expected patterns:
    /// Horizontal line
    /// Vertical line
    /// Olhar para o que se faz no final de cada padrão: virar à esquerda,ou à direita?
    /// 
    /// </summary>
    public void LoadHint(RootBlox rootBlox)
    {

        // 1. Check for mandatory bloxes not used
        List<Tuple<Type, int>> usedBloxTypeCount = rootBlox.GetAllBloxesBellow().GroupBy(b => b.GetType()).Select(g => new Tuple<Type, int>(g.First().GetType(), g.Count())).ToList();

        List<ExpectedBlox> mandatoryBloxesNotUsed = LevelConfiguration.MandatoryBloxes.Where(mb => !usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType && b.Item2 >= mb.MinimumQuantity)).ToList();

        // 1. Check for optional bloxes not used
        List<ExpectedBlox> expectedOptionalBloxesNotUsed = LevelConfiguration.OptionalExpectedBloxes.Where(mb => !usedBloxTypeCount.Exists(b => b.Item1 == mb.BloxType && b.Item2 >= mb.MinimumQuantity)).ToList();




    }

    /// <summary>
    /// We are assuming this function is called after a success execution
    /// 
    /// Basically we have to read 
    /// -Straight lines (either up or down)
    /// -Turns
    /// -If a straight lines does not lead either to a left or a right, consider like the previous ones
    /// 
    /// - how an user dealed with climb up and climb down?
    /// 
    /// We have to check how the user implemented those patterns
    /// 
    /// For example, making three lines with the same size, may have a Turn left or right based on if iteration equals a concrete number
    /// or if an iteration number is a multiple of some number
    /// 
    /// 
    /// Other example,  making four lines, two of the same size, and two not of the same size. The user could have three for cycles, one for the first
    /// two lines, and the others for the remaining lines. Or the user could have a bunch of walks and turns. Or the user could have one big cycle, 
    /// with a bunch of if's for each turn.
    /// 
    /// 
    /// When we implement the AI we need to look at the closest patterns and get the synthesized solution
    /// 
    /// Maybe we just start reading patterns in "for" levels and check how the user solved them?
    /// 
    /// -Sequences of Walks and Climbs
    /// -Cycles -> how many? one for each, or one for every?
    /// -
    /// 
    /// To read same lines we go to mandatory steps and check sequences of the same X and same Y, and same count
    /// 
    /// </summary>
    public void ReadPatterns()
    {
        //LevelConfiguration.MandatorySteps.GroupBy(step => step.CoordinateInPlot.y).Select(step => new { count = step.Count(), });

        //First we read line sequences
        ObjectiveStep previousStep = null;
        int lineSizeCount = 0;
        bool followingX = false;
        foreach(ObjectiveStep step in LevelConfiguration.MandatorySteps)
        {
            if(previousStep == null)
            {
                previousStep = step;
                lineSizeCount += 1;
            }
            else
            {
                followingX = step.CoordinateInPlot.x == previousStep.CoordinateInPlot.x;
            }
        }

    }

    #endregion

    #region Character handling
    public void SetCharacter()
    {
        CubotController cubot = LevelConfiguration.Character;
        cubot.SetUp(LevelConfiguration, tilesInScene);
    }


    #endregion

}

