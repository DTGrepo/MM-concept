using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    [Header("Map Settings")] [SerializeField]
    private int height;

    [SerializeField] private int width;
    [SerializeField] private int n = 9;
    [SerializeField] int minHeight = 0;
    [SerializeField] int maxHeight = 9;
    [SerializeField] private int roughness = 3;

    [Header("Tilemaps")] [SerializeField] private Tilemap groundMap;
    [SerializeField] private Tilemap vegetationMap;
    [SerializeField] private Tilemap interactablesMap;

    [Header("Controls")] [SerializeField] public Button generateHeightmapButton;
    [SerializeField] public Button diamondSquareButton;
    [SerializeField] public Button medianFilterButton;
    [SerializeField] public Button renderHeightmapButton;
    [SerializeField] public Button renderHeatmapButton;
    [SerializeField] public Button generateMapTilesButton;


    [SerializeField] private TileDatas tileset;
    [SerializeField] private Grid grid;

    int mapSideLength;
    private int[,] heightMap;

    void Start()
    {
        initControls();

        // mapSideLength = (int)Math.Pow(2, n)+1;
        heightMap = new int[mapSideLength, mapSideLength];
        // heightMap = ShatteredPlains(0, 0);
        mainRender(heightMap, mapSideLength);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HightlightTileAtMouse();
        }
    }

    private void initControls()
    {
        this.grid = grid.GetComponent<Grid>();

        Button generateHeightmapButton = this.generateHeightmapButton.GetComponent<Button>();
        generateHeightmapButton.onClick.AddListener(OnClickGenerateButton);

        Button diamondSquareButton = this.diamondSquareButton.GetComponent<Button>();
        diamondSquareButton.onClick.AddListener(OnClickDiamondSquareButton);

        Button medianFilterButton = this.medianFilterButton.GetComponent<Button>();
        medianFilterButton.onClick.AddListener(OnClickMedianFilterButton);

        Button renderHeightmapButton = this.renderHeightmapButton.GetComponent<Button>();
        renderHeightmapButton.onClick.AddListener(OnClickRenderHeightmap);

        Button renderHeatmapButton = this.renderHeatmapButton.GetComponent<Button>();
        renderHeatmapButton.onClick.AddListener(OnClickRenderHeatmap);

        Button generateMapTilesButton = this.generateMapTilesButton.GetComponent<Button>();
        generateMapTilesButton.onClick.AddListener(OnClickGenerateMapTiles);
    }

    void OnClickGenerateButton()
    {
        mapSideLength = (int)Math.Pow(2, n) + 1;
        heightMap = GenerateNewHeightMap(mapSideLength, roughness);
        mainRender(heightMap, mapSideLength);
    }

    void OnClickDiamondSquareButton()
    {

        PerformDiamondSquareSmoothing(heightMap, mapSideLength, roughness);
        mainRender(heightMap, mapSideLength);
    }

    void OnClickMedianFilterButton()
    {
        ApplyMedianFilterToMap(heightMap, mapSideLength);
        mainRender(heightMap, mapSideLength);
    }

    void OnClickGenerateMapTiles()
    {
        int tempN = 8;
        mapSideLength = (int)Math.Pow(2, tempN) + 1;
        heightMap = GenerateNewHeightMap(mapSideLength, 20);
        RenderHeightMap(heightMap, mapSideLength, -256, 0);
        // groundMap.RefreshAllTiles();
        // Canvas.ForceUpdateCanvases();
        // LayoutRebuilder.ForceRebuildLayoutImmediate(groundMap as RectTransform);

        heightMap = GenerateNewHeightMap(mapSideLength, 30);
        RenderHeightMap(heightMap, mapSideLength, 0, 0);

        heightMap = GenerateNewHeightMap(mapSideLength, 40);
        RenderHeightMap(heightMap, mapSideLength, -256, -256);

        heightMap = GenerateNewHeightMap(mapSideLength, 50);
        RenderHeightMap(heightMap, mapSideLength, 0, -256);
    }

    public int[,] ShatteredPlains(int w, int h)
    {
        minHeight = 7;
        mapSideLength = (int)Math.Pow(2, n) + 1;
        heightMap = GenerateNewHeightMap(mapSideLength, roughness);

        //do some shit
        int startY = Random.Range(0, mapSideLength);
        int startX = 0;

        formCrack(startX, startY);


        return heightMap;
    }

    private void formCrack(int x, int y)
    {
        Vector3Int currentCell = new Vector3Int(x, y, 0);
        
        if (x-1 > 0 && x+1 < mapSideLength && y-1 > 0 && y+1 < mapSideLength) //if within the X and Y bounds
        {
            Vector3Int? lowestNeigbor = getLowestNeighboringCell(currentCell);
            
        }
        
    }

    /**
     * By nature this checks surrounding tiles from top left to bottom right, and rivers will favor a certain direction
     * when there are multiple lower tiles
     */
    private Vector3Int? getLowestNeighboringCell(Vector3Int currentCell)
    {
        Vector3Int lowestNeighbor;
        int x = currentCell.x;
        int y = currentCell.y;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if(i == 0 && j == 0){}
                else{
                    if (heightMap[x+i,y+j] < heightMap[x, y])
                    {
                        return new Vector3Int(x + i, y + j, 0);
                    }
                }
            }
        }
        
        return null;
    }

    public int[,] GenerateNewHeightMap(int mapSideLength, int roughness)
    {

        int[,] newMap = new int[mapSideLength, mapSideLength];

        Debug.Log("HeightMapDimensions: "+mapSideLength+"x"+mapSideLength);
        //set 4 corners to random value
        newMap[0, 0] = Random.Range(minHeight, maxHeight);
        newMap[0, mapSideLength-1] = Random.Range(minHeight, maxHeight);
        newMap[mapSideLength-1, mapSideLength-1] = Random.Range(minHeight, maxHeight);
        newMap[mapSideLength-1, 0] = Random.Range(minHeight, maxHeight);


        PerformDiamondSquare(newMap, mapSideLength, roughness);
        
        // PrintHeightMap(heightMap, mapSideLength);

        int chunkSize = mapSideLength - 1;
        int currentRoughness = roughness;
        while (chunkSize > 1)
        {
            var half = chunkSize / 2;
            // squareStep();
            for (int y = 0; y < mapSideLength-1; y+=chunkSize)
            {
                for (int x = 0; x < mapSideLength-1; x+=chunkSize)
                {
                    newMap[y+half, x+half] = ((newMap[y, x] +
                                               newMap[y, x + chunkSize] +
                                               newMap[y + chunkSize, x] +
                                               newMap[y + chunkSize, x + chunkSize])
                                              / 4) + Random.Range(currentRoughness, -currentRoughness);
                }
            }
            
            // PrintHeightMap(heightMap, mapSideLength);
            
            // diamondStep();
            for (int y = 0; y < mapSideLength; y+=half)
            {
                for (int x = (y+half)%chunkSize; x < mapSideLength; x+=chunkSize)
                {
                    int sum = 0, count = 0;
                    String sumString = "";
                    if (x-half >= 0)
                    { // if there is a value to the left...
                        sum += newMap[y, x - half];
                        // sumString += heightMap[x - half, x];
                        count++;
                    }
                    if (x+half < mapSideLength)
                    {// if there is a value to the right...
                        sum += newMap[y, x + half];
                        count++;
                    }
                    if (y-half >= 0)
                    {// if there is a value below...
                        sum += newMap[y - half, x];
                        count++;
                    }
                    if (y+half < mapSideLength)
                    {// if there is a value above...
                        sum += newMap[y + half, x];
                        count++;
                    }
                    
                    newMap[y,x] = (sum/count)+ Random.Range(-currentRoughness, currentRoughness);
                }
            }
            
            // PrintHeightMap(heightMap, mapSideLength);
            
            chunkSize /= 2;
            currentRoughness /= 2;
        }
        
        return newMap;
    }
    
    public int[,] PerformDiamondSquare(int[,] map, int mapSideLength, int roughness)
    {

        int chunkSize = mapSideLength - 1;
        int currentRoughness = roughness;
        while (chunkSize > 1)
        {
            var half = chunkSize / 2;
            
            // squareStep
            for (int y = 0; y < mapSideLength-1; y+=chunkSize)
            {
                for (int x = 0; x < mapSideLength-1; x+=chunkSize)
                {
                    map[y+half, x+half] = ((map[y, x] +
                                            map[y, x + chunkSize] +
                                            map[y + chunkSize, x] +
                                            map[y + chunkSize, x + chunkSize])
                                           / 4) + Random.Range(currentRoughness, -currentRoughness);
                }
            }

            // diamondStep
            for (int y = 0; y < mapSideLength; y+=half)
            {
                for (int x = (y+half)%chunkSize; x < mapSideLength; x+=chunkSize)
                {
                    int sum = 0, count = 0;
                    if (x-half >= 0)
                    { // if there is a value to the left...
                        sum += map[y, x - half];
                        count++;
                    }
                    if (x+half < mapSideLength)
                    {// if there is a value to the right...
                        sum += map[y, x + half];
                        count++;
                    }
                    if (y-half >= 0)
                    {// if there is a value below...
                        sum += map[y - half, x];
                        count++;
                    }
                    if (y+half < mapSideLength)
                    {// if there is a value above...
                        sum += map[y + half, x];
                        count++;
                    }
                    
                    map[y,x] = (sum/count)+ Random.Range(-currentRoughness, currentRoughness);
                }
            }

            chunkSize /= 2;
            currentRoughness /= 2;
        }
        
        ApplyMedianFilterToMap(map, mapSideLength);
        return map;
    }
    
    public int[,] PerformDiamondSquareSmoothing(int[,] map, int mapSideLength, int roughness)
    {

        int chunkSize = mapSideLength - 1;
        int currentRoughness = roughness;
        while (chunkSize > 1)
        {
            var half = chunkSize / 2;
            
            // squareStep
            for (int y = 0; y < mapSideLength-1; y+=chunkSize)
            {
                for (int x = 0; x < mapSideLength-1; x+=chunkSize)
                {
                    map[y+half, x+half] = ((map[y+half, x+half]+
                                            map[y, x] +
                                            map[y, x + chunkSize] +
                                            map[y + chunkSize, x] +
                                            map[y + chunkSize, x + chunkSize])
                                           / 5) + Random.Range(currentRoughness, -currentRoughness);
                }
            }

            // diamondStep
            for (int y = 0; y < mapSideLength; y+=half)
            {
                for (int x = (y+half)%chunkSize; x < mapSideLength; x+=chunkSize)
                {
                    int sum = map[y,x], count = 1;
                    if (x-half >= 0)
                    { // if there is a value to the left...
                        sum += map[y, x - half];
                        count++;
                    }
                    if (x+half < mapSideLength)
                    {// if there is a value to the right...
                        sum += map[y, x + half];
                        count++;
                    }
                    if (y-half >= 0)
                    {// if there is a value below...
                        sum += map[y - half, x];
                        count++;
                    }
                    if (y+half < mapSideLength)
                    {// if there is a value above...
                        sum += map[y + half, x];
                        count++;
                    }
                    
                    map[y,x] = (sum/count)+ Random.Range(-currentRoughness, currentRoughness);
                }
            }

            chunkSize /= 2;
            currentRoughness /= 2;
        }
        ApplyMedianFilterToMap(map, mapSideLength);
        return map;
    }

    private void ApplyMedianFilterToMap(int[,] map, int mapSideLength)
    {
        for (int x = 1; x < mapSideLength-1; x++)
        {
            for (int y = 1; y < mapSideLength-1; y++)
            {
                int[] vals =
                {
                    map[x-1,y+1], map[x,y+1], map[x+1,y+1],
                    map[x-1,y  ], map[x,y  ], map[x+1,y  ],
                    map[x-1,y-1], map[x,y-1], map[x+1,y-1]
                };
                Array.Sort(vals);
                map[x, y] = vals[4]; //get the median value
            }
        }
        
    }
    

    private void PrintHeightMap(int[,] map, int heightMapSize)
    {
        String mapString = "";
        for (int y = 0; y < heightMapSize; y++)
        {
            for (int x = 0; x < heightMapSize; x++)
            {
                mapString += (map[y, x] + "  ");
            }

            mapString += "\n";
        }

        Debug.Log(mapString);
    }
    
    private void mainRender(int[,] heightMap, int heightMapSize)
    {
        RenderHeightMap(heightMap, heightMapSize, -256, -256);
    }
    
    void OnClickRenderHeightmap()
    {
        //switch tileset to heightmap
        tileset = Resources.Load<TileDatas>("TileDatas/SatelliteTileDatas");
        minHeight = 0;
        maxHeight = 19;
        mainRender(heightMap, mapSideLength);
    }
    
    void OnClickRenderHeatmap()
    {
        //switch tileset to heatmap
        tileset = Resources.Load<TileDatas>("TileDatas/TopographyTileDatas");
        minHeight = 0;
        maxHeight = 19;
        mainRender(heightMap, mapSideLength);
    }
    
    private void RenderHeightMap(int[,] heightMap, int heightMapSize, int startX, int startY)
    {
        Debug.Log("+++ Rendering Heightmap +++");
        for (int x = 0; x < heightMapSize; x++)
        {
            for (int y = 0; y < heightMapSize; y++)
            {
                groundMap.SetTile(new Vector3Int(x+startX, y+startY, 0), tileset.tiles[Math.Clamp(heightMap[x, y], minHeight, maxHeight)]);
            }
        }
    }


    private void HightlightTileAtMouse()
    {
        // save the camera as public field if you using not the main camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // get the collision point of the ray with the z = 0 plane
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = grid.WorldToCell(worldPoint);
        // groundMap.GetTile(position);
        groundMap.SetTile(position, Resources.Load<TileDatas>("TileDatas/TopographyTileDatas").tiles[19]);
        // return position;
    }
    
    
    private int CheckMooreCells(int x, int y, Tilemap tileMap, List<TileBase> targetList)
    {
        int totalSimilarTiles = 0;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if(i == 0 && j == 0){}
                else{
                    TileBase currentTile = tileMap.GetTile(new Vector3Int(x + i, y + j, 0));
                    if (targetList.Contains(currentTile) && currentTile != null) totalSimilarTiles++;
                }
            }
        }
        return totalSimilarTiles;
    }
    
}