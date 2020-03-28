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
        public MovementType movementType;
        public PlotTile destinyTile;
    }

    public bool ExecutingAction { get; set; }


    private MovementDestination movement;

    private List<PlotTile> tilesInScene;

    private LevelDescriptor levelDescriptor;
    private PlotTile CurrentTile { get; set; }
    private CharacterOrientation CurrentOrientation { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        movement.destinyTile = null;
        movement.movementType = MovementType.NONE;

    }

    // Update is called once per frame
    void Update()
    {
        float speed = 20f;
        // If a forward movement is being called
        if (movement.movementType == MovementType.FORWARD)
        {
            ExecutingAction = true;
            Vector3 destinyPosition = movement.destinyTile.transform.position;
            destinyPosition.y = this.transform.position.y; //We are moving the character over a Tile, and not to the center of the tile

            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, step);

            if (Vector3.Distance(transform.position, destinyPosition) < 0.1f)
            {
                ExecutingAction = false;
                movement.destinyTile = null;
                movement.movementType = MovementType.NONE;
            }
        }

    }

    /// <summary>
    /// Calls a forward movement
    /// The movem
    /// </summary>
    /// <param name="destinyTile"></param>
    public void MoveForward()
    {
        // Validates if movement is possible and throws an exception if it isn't
        ValidateForwardMovement();

        // Gets the tile it is going to move to

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
        PlotTile nextTile = tilesInScene.Where(t => t.PlotPosition == nextPlotTileCoordinates).FirstOrDefault();

        if (nextTile == null) throw new HoleAheadException();

        movement.movementType = MovementType.FORWARD;
        movement.destinyTile = nextTile;

    }

    /// <summary>
    /// Validates if there is an obstacle ahead
    /// Throws an exception if there is
    /// </summary>
    private void ValidateForwardMovement()
    {
        Vector3 tileExtents = CurrentTile.GetComponent<Renderer>().bounds.extents;
        float rayDistance = 0;

        switch (CurrentOrientation)
        {
            case CharacterOrientation.NORTH:
            case CharacterOrientation.SOUTH:
                rayDistance = tileExtents.z * 1.25f;
                break;
            case CharacterOrientation.EAST:
            case CharacterOrientation.WEST:
                rayDistance = tileExtents.x * 1.25f;
                break;
        }

        // Verifies if there is an obstacle ahead of this character
        if (Physics.Raycast(transform.position, Vector3.forward, rayDistance))
        {
            throw new ObstacleAheadException();
        }
    }


    /// <summary>
    /// Sets the initial position of the cubot
    /// </summary>
    /// <param name="initialPlotTile"></param>
    /// <param name="initialOrientation"></param>
    public void SetInitalPosition(LevelDescriptor configuration, List<PlotTile> tilesInScene)
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

        CurrentTile = initialPlotTile;
        CurrentTile.Step();
    }
}
