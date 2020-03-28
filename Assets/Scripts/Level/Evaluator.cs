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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                
                PlotTile newTile = Instantiate(templateTile);
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

        //destroy the templatetile

    }

}

