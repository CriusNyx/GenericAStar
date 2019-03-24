using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wintellect.PowerCollections;

public static class AStar
{
    public static T[] Find<T>(T start, T end, Func<T, T, float> getHScore, Func<AStarScore<T> ,T, float> getGScore, bool returnPathOnFail = true) where T : class, AStarNode
    {
        if(start == null || end == null)
        {
            return new T[] { };
        }

        //Check easy case
        if(start == end)
        {
            return new T[] { start };
        }

        //Data structures
        OrderedBag<AStarScore<T>> openList = new OrderedBag<AStarScore<T>>((x, y) => x.f.CompareTo(y.f));
        Dictionary<T, bool> visited = new Dictionary<T, bool>();
        Dictionary<T, AStarScore<T>> closedList = new Dictionary<T, AStarScore<T>>();

        T bestHSoFar = null;
        float bestHScoreSoFar = Mathf.Infinity;

        //initialize algorithm
        //Enclosed in a block for scoping
        {
            AStarScore<T> score = new AStarScore<T>(start, null, getHScore(start, end), 0f);
            openList.Add(score);
            visited.Add(start, true);
            bestHSoFar = start;
            bestHScoreSoFar = score.h;
        }

        //While there are entities in the open list
        while(openList.Count > 0)
        {
            //pop the open list
            AStarScore<T> current = openList.RemoveFirst();

            //add the scanned node to the closed list
            closedList.Add(current.node, current);

            //scan adjacent
            foreach(T adjacent in current.node.adjacent)
            {
                //check if it's the end
                if(adjacent == end)
                {
                    AStarScore<T> last = new AStarScore<T>(end, current.node, 0f, 0f);
                    return Trace(last, closedList);
                }
                //check if the object is node in the closed list
                //if it is, update it
                if(closedList.ContainsKey(adjacent))
                {
                    //get the score currently in the closed list
                    AStarScore<T> adjacentScore = closedList[adjacent];
                    //the potential g score this could acheive
                    float potentialG = getGScore(current, adjacent);
                    //if the potential gScore is lower then the current gScore, update the node
                    if(potentialG < adjacentScore.g)
                    {
                        closedList[adjacent] = new AStarScore<T>(adjacent, current.node, adjacentScore.h, potentialG);
                    }
                }
                //If this hasn't been visited, mark it visited and add it to the open list
                else if(!visited.ContainsKey(adjacent))
                {
                    //calculate the score
                    AStarScore<T> score = new AStarScore<T>(adjacent, current.node, getHScore(adjacent, end), getGScore(current, adjacent));

                    //mark adjacent as visited
                    visited.Add(adjacent, true);
                    //add the score to the open list
                    openList.Add(score);

                    if(score.h < bestHScoreSoFar)
                    {
                        bestHSoFar = adjacent;
                        bestHScoreSoFar = score.h;
                    }
                }
                //add the new score to the closed list
            }
        }
        if(returnPathOnFail)
        {
            return Trace(closedList[bestHSoFar], closedList);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Trace a path back to the start
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="score"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    private static T[] Trace<T>(AStarScore<T> score, Dictionary<T, AStarScore<T>> dic) where T : class, AStarNode
    {
        LinkedList<T> output = new LinkedList<T>();
        for(AStarScore<T> current = score; current != null; current = current.parent == null ? null : dic[current.parent])
        {
            output.AddFirst(current.node);
        }
        return output.ToArray();
    }
}