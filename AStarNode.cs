using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AStarNode
{
    IEnumerable<AStarNode> adjacent { get; }
}