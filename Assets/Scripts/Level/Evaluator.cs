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
public class Evaluator : MonoBehaviour
{
    private DateTime startTime;
    private List<PlotTile> tilesInScene = new List<PlotTile>();

    [SerializeField] LevelDescriptor LevelConfiguration;


    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;
        BuildTilePlot();
        SetCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        for (int plotY = 0; plotY<LevelConfiguration.PlotHeight; plotY++) //iterates through lines (y)
        {
            for(int plotX = 0; plotX < LevelConfiguration.PlotWidth; plotX++) //iterates through columns (x)
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

    public void SetCharacter()
    {
        CubotController cubot = LevelConfiguration.Character;
        Vector3 cubotExtents = cubot.GetComponent<Renderer>().bounds.extents;
        Vector3Int cubotPlotInitialPosition = LevelConfiguration.CharacterStartPointInPlot;
        float rotY = (int)LevelConfiguration.CharacterInitialOrientation;
        PlotTile initialPlotTile = tilesInScene.Where(t => t.PlotPosition == cubotPlotInitialPosition).First();
        Vector3 initialPlotTileExtents = initialPlotTile.GetComponent<Renderer>().bounds.extents;

        //Cubot must be over an initial plot tile, so we are going to position it
        Vector3 initialPosition = initialPlotTile.transform.position;
        initialPosition.y += cubotExtents.y + initialPlotTileExtents.y;

        Vector3 cubotInitialRotation = new Vector3(0, rotY, 0);

        cubot.transform.position = initialPosition;
        cubot.transform.rotation = Quaternion.Euler(cubotInitialRotation);

    }

}

