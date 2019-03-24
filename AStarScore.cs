using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarScore<T> where T : AStarNode
{
    public readonly T node, parent;

    public readonly float h, g, f;

    public AStarScore(T node, T parent, float h, float g)
    {
        this.node = node;
        this.parent = parent;
        this.h = h;
        this.g = g;
        f = h + g;
    }
}
