using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarDebug : MonoBehaviour
{
    Room[,] rooms;
    List<Room> roomItterable;
    Room[] path;
    GameObject start, end;

    Vector3 startLastFrame, endLastFrame;

    void Start()
    {
        Generate();
        start = new GameObject("Start");
        end = new GameObject("End");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Generate();
        }
        if(start.transform.position != startLastFrame || end.transform.position != endLastFrame)
        {
            startLastFrame = start.transform.position;
            endLastFrame = end.transform.position;

            var startRoom = roomItterable.FirstOrDefault(x => x.ContainsPoint(start.transform.position));
            var endRoom = roomItterable.FirstOrDefault(x => x.ContainsPoint(end.transform.position));
            if(startRoom != null && endRoom != null)
            {
                path = Room.GetPath(startRoom, endRoom);
                
            }
            else
            {
                Debug.Log("Failed");
            }
        }
        if(path != null)
        {
            for(int i = 0; i < path.Length - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                Debug.DrawLine(a.transform.position, b.transform.position, Color.yellow);
            }
        }
    }

    void Generate()
    {
        //clear existing rooms
        if(rooms != null)
        {
            foreach(var room in rooms)
            {
                Destroy(room.gameObject);
            }
        }

        //create a new array for rooms
        rooms = new Room[100,100];
        roomItterable = new List<Room>();

        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                if(Random.value > 0.25f)
                {
                    Room room = new GameObject("Rooms").AddComponent<Room>();
                    room.size = Vector2.one;
                    room.transform.position = Vector3.right * 2.1f * i + Vector3.forward * 2.1f * j;
                    room.transform.parent = transform;
                    rooms[i, j] = room;
                    roomItterable.Add(room);
                }
            }
        }

        //link rooms
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                Room room = rooms[i, j];
                if(room == null)
                {
                    continue;
                }
                //link right
                if(i != 99)
                {
                    Room right = rooms[i + 1, j];
                    if(right != null)
                    {
                        room.adj.Add(right);
                        right.adj.Add(room);
                    }
                }
                //link forward
                if(j != 99)
                {
                    Room forward = rooms[i, j + 1];
                    if(forward != null)
                    {
                        room.adj.Add(forward);
                        forward.adj.Add(room);
                    }
                }
            }
        }
    }
}