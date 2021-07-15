using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;
    [SerializeField] Vector2Int destinationCoordinates;

    Node startNode;
    Node destinationNode;
    Node currentSearchNode;

    //Frontier that we are yet to explore (queue neighbors).
    Queue<Node> frontier = new Queue<Node>();
    //To see if a node has already been explored or not.
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();
    


    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();

        if(gridManager != null)
        {
            grid = gridManager.Grid;
        }

    }

    void Start()
    {
        startNode = gridManager.Grid[startCoordinates];
        destinationNode = gridManager.Grid[destinationCoordinates];

        BreathFirstSearch();
        BuildPath();
    }

    void ExploringNeighbors()
    {
        List<Node> neighbors = new List<Node>();
        //Obtains the neighbors from the current Search Node.
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborCoordinates = direction + currentSearchNode.coordinates;

            if (grid.ContainsKey(neighborCoordinates))
            {
                neighbors.Add(grid[neighborCoordinates]);

            }

        }
        //Cheks for all neighbors, if they can be added to queue.
        foreach(Node neighbor in neighbors)
        {
            if(!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                //Create conexion in the search tree.
                neighbor.connectedTo = currentSearchNode;
                //We add the node to the dictionary to avoid been added to the queue again.
                reached.Add(neighbor.coordinates, neighbor);
                //We queue the node in order to wait its turn to be the currenSearchNode.
                frontier.Enqueue(neighbor);
            }
        }
    }

    void BreathFirstSearch()
    {
        bool isRunning = true;

        frontier.Enqueue(startNode);
        reached.Add(startCoordinates, startNode);

        while(frontier.Count > 0 && isRunning)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            //Find neighbors from current Search Node.
            ExploringNeighbors();
            if(currentSearchNode.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while(currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }

}
