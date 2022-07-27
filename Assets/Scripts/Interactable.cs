using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public abstract class Interactable
{
    public abstract Sprite GetSprite();
    public abstract Vector3Int GetPosition();
    public abstract void OnSelected();
}
