using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character :  Interactable
{

    private Sprite Sprite { get; set; }
    private Vector3Int Position{ get; set; }
    private string Name{ get; set; }
    private int Speed{ get; set; }

    public Character(string name, int speed, Sprite sprite, Vector3Int position)
    {
        this.Name = name;
        this.Speed = speed;
        this.Sprite = sprite;
        this.Position = position;
    }
    
    public override void OnSelected()
    {
        Debug.Log("Character Selected: "+Name);
        Debug.Log("# of Viable Moves: "+GetViableMoves().Count);
        //todo: show characters movement range
    }

    public List<Vector3Int> GetViableMoves()
    {
        List<Vector3Int> viableMoves = new List<Vector3Int>();
        RecursiveFindViableMoves(viableMoves,Position, Speed);
        return viableMoves;
    }

    private void RecursiveFindViableMoves(List<Vector3Int> viableMoves, Vector3Int currentPosition, int remainingMovement)
    {
        //add current tile if not already in the list && not the position the Character is already in
        if (!viableMoves.Contains(currentPosition) && currentPosition != Position)
            viableMoves.Add(currentPosition);

        //exit condition
        if (remainingMovement == 0)
            return;

        //check up
        RecursiveFindViableMoves(viableMoves, currentPosition+Vector3Int.up, remainingMovement-1);
        //check down
        RecursiveFindViableMoves(viableMoves, currentPosition+Vector3Int.down, remainingMovement-1);
        //check left
        RecursiveFindViableMoves(viableMoves, currentPosition+Vector3Int.left, remainingMovement-1);
        //check right
        RecursiveFindViableMoves(viableMoves, currentPosition+Vector3Int.right, remainingMovement-1);
    }

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override Vector3Int GetPosition()
    {
        return Position;
    }
    
    
}