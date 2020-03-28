using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static LevelDescriptor;

public class CubotController : MonoBehaviour
{
    enum MovementType
    {
        NONE,
        FORWARD,
        CLIMB_UP,
        CLIMB_DOWN
    }

    struct MovementDestination
    {
        // The phases will be executed in order. The last one shall always be NONE
        public List<MovementType> movementPhases;
        public PlotTile destinyTile;
    }
    public float Speed { get; set; }
    public bool ExecutingAction { get; set; }

    private MovementDestination destination;

    private List<PlotTile> tilesInScene;

    private LevelDescriptor levelDescriptor;
    private PlotTile CurrentTile { get; set; }
    private CharacterOrientation CurrentOrientation { get; set; }

    // Start is called before the first frame update
    void Start()
    {


    }

    private void ResetMovement()
    {
        destination.destinyTile = null;
        destination.movementPhases = new List<MovementType>() {MovementType.NONE };
    }

    // Update is called once per frame
    void Update()
    {        
        float speed = Speed;
        MovementType nextMove = destination.movementPhases[0];

        if(nextMove == MovementType.NONE)
        {
            destination.destinyTile = null;
            destination.movementPhases = new List<MovementType>() { MovementType.NONE };
            ExecutingAction = false;
        }
        else
        {
            ExecutingAction = true;
            Vector3 tileExtents = destination.destinyTile.GetComponent<Renderer>().bounds.extents;
            Vector3 cubotExtents = this.GetComponent<Renderer>().bounds.extents;
            Vector3 destinyPosition = new Vector3();
            switch (nextMove)
            {
                case MovementType.FORWARD:
                    destinyPosition = destination.destinyTile.transform.position;
                    destinyPosition.y = this.transform.position.y; //We are moving the character over a Tile, and not to the center of the tile
                    break;
                case MovementType.CLIMB_UP: //Climb up phase consist only in elevating the character. It is supposed to call a forward after this
                case MovementType.CLIMB_DOWN: //Climb down phase consist only in landing the character. It is supposed to have a forward call before
                    destinyPosition = this.transform.position;
                    destinyPosition.y = destination.destinyTile.transform.position.y + tileExtents.y + cubotExtents.y;
                    break;
            }
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, step);

            if (Vector3.Distance(transform.position, destinyPosition) < 0.1f)
            {
                destination.movementPhases.RemoveAt(0);
            }

        }

        // If a forward movement is being called
        /*if (nextMove == MovementType.FORWARD)
        {
            ExecutingAction = true;
            Vector3 destinyPosition = destination.destinyTile.transform.position;
            destinyPosition.y = this.transform.position.y; //We are moving the character over a Tile, and not to the center of the tile

            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, step);

            if (Vector3.Distance(transform.position, destinyPosition) < 0.1f)
            { 
                destination.movementPhases.RemoveAt(0);
            }
        }

        // This will engage two phases. First it goes up, and then sets a forward movement
        if(nextMove == MovementType.CLIMB_UP)
        {
            Vector3 tileExtents = destination.destinyTile.GetComponent<Renderer>().bounds.extents;
            Vector3 cubotExtents = this.GetComponent<Renderer>().bounds.extents;

            ExecutingAction = true;
            Vector3 destinyPosition = this.transform.position;
            destinyPosition.y = destination.destinyTile.transform.position.y + tileExtents.y + cubotExtents.y;
            float step = speed * Time.deltaTime; // calculate distance to move

            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, step);

            if (Vector3.Distance(transform.position, destinyPosition) < 0.1f)
            {
                destination.movementPhases.RemoveAt(0);
            }

        }*/
    }

    /// <summary>
    /// Calls a forward movement
    /// The movem
    /// </summary>
    /// <param name="destinyTile"></param>
    public void MoveForward()
    {

        // Gets the tile it is going to move to
        PlotTile nextTile = GetTileAhead();

        if (nextTile == null) throw new NoTileAheadException();

        // Validates if there is a next tile, and its not bellow the current level
        if (nextTile.PlotPosition.z < CurrentTile.PlotPosition.z) throw new HoleAheadException();

        // Validates if there is an obstacle ahed
        if (nextTile.PlotPosition.z > CurrentTile.PlotPosition.z) throw new ObstacleAheadException();

        destination.movementPhases = new List<MovementType>() { MovementType.FORWARD, MovementType.NONE };
        destination.destinyTile = nextTile;
    }

    /// <summary>
    /// Calls a forward movement to climb up
    /// The movement will consist on the character flying up and then going forward
    /// </summary>
    /// <param name="destinyTile"></param>
    public void ClimbUp()
    {
        // Gets the tile it is going to move to
        PlotTile nextTile = GetTileAhead();

        if (nextTile == null) throw new NoTileAheadException();

        // Validates if there is a next tile, and its not bellow the current level
        if  (nextTile.PlotPosition.z < CurrentTile.PlotPosition.z) throw new HoleAheadException();

        //Throws exception if no obstacle ahead
        if (nextTile.PlotPosition.z <= CurrentTile.PlotPosition.z) throw new NoObstacleAheadException();


        destination.movementPhases = new List<MovementType>() { MovementType.CLIMB_UP, MovementType.FORWARD, MovementType.NONE };
        destination.destinyTile = nextTile;
    }

    /// <summary>
    /// Calls a forward movement to climb down
    /// The movement will consist on the character flying up and then going forward
    /// </summary>
    /// <param name="destinyTile"></param>
    public void ClimbDown()
    {
        // Gets the tile it is going to move to
        PlotTile nextTile = GetTileAhead();

        if (nextTile == null) throw new NoTileAheadException();

        // Validates if the next tile bellow the current level
        if (nextTile.PlotPosition.z >= CurrentTile.PlotPosition.z) throw new NoHoleAheadException();

        destination.movementPhases = new List<MovementType>() { MovementType.FORWARD, MovementType.CLIMB_DOWN, MovementType.NONE };
        destination.destinyTile = nextTile;
    }


    /// <summary>
    /// Gets the tile in front of this object
    /// </summary> 
    /// <returns></returns>
    public PlotTile GetTileAhead()
    {
        //First we get the translation in terms of PlotTile coordinates
        Vector3Int translationInPlot = new Vector3Int(0, 0, 0);
        switch (CurrentOrientation)
        {
            case CharacterOrientation.NORTH:
                translationInPlot.y = 1;
                break;
            case CharacterOrientation.SOUTH:
                translationInPlot.y = -1;
                break;
            case CharacterOrientation.EAST:
                translationInPlot.x = 1;
                break;
            case CharacterOrientation.WEST:
                translationInPlot.x = -1;
                break;
        }
        // Then we get the next tile coordinates
        Vector3Int nextPlotTileCoordinates = CurrentTile.PlotPosition + translationInPlot;
        // Then we set movement to next tile
        PlotTile nextTile = tilesInScene.Where(t => t.PlotPosition.x == nextPlotTileCoordinates.x && t.PlotPosition.y == nextPlotTileCoordinates.y).FirstOrDefault();

        return nextTile;
    }

    /// <summary>
    /// Sets the initial position and the bot settings of the cubot
    /// </summary>
    /// <param name="initialPlotTile"></param>
    /// <param name="initialOrientation"></param>
    public void SetUp(LevelDescriptor configuration, List<PlotTile> tilesInScene)
    {
        this.tilesInScene = tilesInScene;
        CurrentOrientation = configuration.CharacterInitialOrientation;
        Vector3 cubotExtents = this.GetComponent<Renderer>().bounds.extents;
        Vector3Int cubotPlotInitialPosition = configuration.CharacterStartPointInPlot;

        PlotTile initialPlotTile = tilesInScene.Where(t => t.PlotPosition == cubotPlotInitialPosition).First();

        float rotY = (int)CurrentOrientation;
        Vector3 initialPlotTileExtents = initialPlotTile.GetComponent<Renderer>().bounds.extents;

        //Cubot must be over an initial plot tile, so we are going to position it
        Vector3 initialPosition = initialPlotTile.transform.position;
        initialPosition.y += cubotExtents.y + initialPlotTileExtents.y;

        Vector3 cubotInitialRotation = new Vector3(0, rotY, 0);

        this.transform.position = initialPosition;
        this.transform.rotation = Quaternion.Euler(cubotInitialRotation);

        Speed = configuration.CharacterSpeed;

        CurrentTile = initialPlotTile;
        CurrentTile.Step();
    }
}
