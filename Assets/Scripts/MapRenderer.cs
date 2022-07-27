using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    
    [SerializeField] private int width = 10;
    [SerializeField] private int length = 20;
    [SerializeField] private Tile tile;
    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tilemap highlightMap;
    [SerializeField] private Tilemap groundMap;

    private List<Interactable> interactableList;

    private Grid grid;
    
    private Sprite blueMarker;
    private Sprite redMarker;
    private Sprite spriteTileHighlight;
    private Sprite spriteViableMoveHighlight;

    private Vector3Int highlightedLocation;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        blueMarker = Resources.Load<Sprite>("Sprites/blue_marker");
        redMarker = Resources.Load<Sprite>("Sprites/red_marker");
        spriteTileHighlight = Resources.Load<Sprite>("Sprites/iso_tile_highlight");
        spriteViableMoveHighlight = Resources.Load<Sprite>("Sprites/iso_tile_viable_move_highlight");

        interactableList = new List<Interactable>();
        RenderMap();
        RenderInteractables();
    }

    private void RenderMap()
    {
        // Tilemap groundMap = GetComponent("groundTilemap") as Tilemap;
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
    
    private void RenderInteractables()
    {
        
        Character newChar1 = new Character("Blueman", 2, blueMarker, new Vector3Int(0, 3, 0));
        interactableList.Add(newChar1);
        Character newChar2 = new Character("Mr. Red", 3, redMarker, new Vector3Int(16, 8, 0));
        interactableList.Add(newChar2);

        foreach (Interactable obj in interactableList)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = obj.GetSprite();
                Tile hTile = ScriptableObject.CreateInstance<Tile>();
                hTile.sprite = spriteTileHighlight;
                interactableMap.SetTile(obj.GetPosition(), tile);
        }
        
        // charList.Add(new Character(new Vector3Int(0,0,0), Resources.Load("/Sprites/red_marker") as Sprite));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //clear all highlights
            highlightMap.ClearAllTiles();
            
            Vector3Int clickedPosition = GetClickedMapPosition();
            List<Interactable> interactablesOnTile = CheckForInteractables(clickedPosition);
            if (interactablesOnTile.Count > 0)
            {
                if (interactablesOnTile[0] is Character)
                {
                    Character c = interactablesOnTile[0] as Character;
                    foreach (Vector3Int viableMovePosition in c.GetViableMoves())
                    {
                        HighlightViableMove(viableMovePosition);
                    }
                }
                interactablesOnTile[0].OnSelected();
            }
            //set main tile highlight
            DrawHighlight(clickedPosition);
        }  
    }

    private List<Interactable> CheckForInteractables(Vector3Int clickedPosition)
    {
        List<Interactable> interactablesOnTile = new List<Interactable>();
        foreach (Interactable interactable in interactableList)
        {
            if (interactable.GetPosition() == clickedPosition)
            {
                interactablesOnTile.Add(interactable);
            }
        }

        return interactablesOnTile;
    }

    private Vector3Int GetClickedMapPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // get the collision point of the ray with the z = 0 plane
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        return grid.WorldToCell(worldPoint);
    }

    private void DrawHighlight(Vector3Int position)
    {
        //create new tile and set the sprite
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = spriteTileHighlight;
        
        //clear the old highlight tile
        highlightMap.SetTile(highlightedLocation, null);
        
        //set the new highlight tile
        highlightMap.SetTile(position, tile);
        
        //update the highlightedLocation
        highlightedLocation = position;
        
    }
    
    private void HighlightViableMove(Vector3Int position)
    {
        //create new tile and set the sprite
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = spriteViableMoveHighlight;

        //set the new highlight tile
        highlightMap.SetTile(position, tile);
    }
}