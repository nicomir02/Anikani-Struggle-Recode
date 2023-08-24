using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public class Node
    {
        public Vector3Int Position;
        public int GScore;
        public int FScore;
        public Node CameFrom;
        public int Cost;

        public Node(Vector3Int position, int cost = 1)
        {
            this.Position = position;
            this.GScore = int.MaxValue;
            this.FScore = int.MaxValue;
            this.Cost = cost;
        }
    }

    public Vector3Int start;
    public Vector3Int target;
    public List<Vector3Int> Path;

    private int allCost = -1;

    private MapBehaviour mapBehaviour;

    public Pathfinding(Vector3Int StartPoint, Vector3Int EndPoint, Unit unit)
    {
        start = StartPoint;
        target = EndPoint;
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
    }

    public int getAllCost() {
        return allCost;
    }

    // Hauptmethode zur Pfadsuche
    public List<Vector3Int> FindPath()
    {
        // Initialisierung der Knotenlisten und des Dictionaries
        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Startknoten initialisieren und zu openList hinzufügen
        Node startNode = new Node(start);
        startNode.GScore = 0;
        startNode.FScore = HeuristicCost(start, target);
        openList.Add(startNode);
        allNodes.Add(start, startNode);

        int testablaeufe = 0;

        // Hauptalgorithmus
        while (openList.Count > 0 && testablaeufe < 25)
        {
            // Finde Knoten in der offenen Liste mit dem niedrigsten F-Score
            Node current = GetLowestFScoreNode(openList);

            // Überprüfe, ob das Ziel erreicht ist
            if (current.Position.Equals(target))
            {
                allCost = current.FScore;
                return ReconstructPath(current);
            }

            // Aktuellen Knoten aus der offenen Liste entfernen und zur geschlossenen Liste hinzufügen
            openList.Remove(current);
            closedList.Add(current);

            // Überprüfe alle Nachbarn des aktuellen Knotens
            foreach (Node neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                // Kosten vom Startknoten zum Nachbarn (über den aktuellen Knoten)
                int tentativeGScore = current.GScore + neighbor.Cost;

                // Falls der Nachbarknoten noch nicht in der offenen Liste ist, füge ihn hinzu
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                // Wenn der neue Pfad nicht kürzer ist, ignoriere diesen Nachbarn
                else if (tentativeGScore >= neighbor.GScore)
                {
                    continue;
                }

                // Dies ist ein besserer Pfad, aktualisiere die Informationen für diesen Nachbarn
                neighbor.CameFrom = current;
                neighbor.GScore = tentativeGScore;
                neighbor.FScore = tentativeGScore + HeuristicCost(neighbor.Position, target);


                
            }

            testablaeufe++;
        }

        // Falls wir hier ankommen, gibt es keinen Pfad zum Ziel
        return null;
    }

    // Methode zur Rückverfolgung des Pfades
    List<Vector3Int> ReconstructPath(Node current)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        while (current != null)
        {
            path.Add(current.Position);
            current = current.CameFrom;
        }
        path.Reverse();
        return path;
    }

    // Methode zur Berechnung der heuristischen Kosten
    int HeuristicCost(Vector3Int start, Vector3Int end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) + Mathf.Abs(start.z - end.z);
    }

    // Methode zur Auswahl des Knotens mit dem niedrigsten F-Score
    Node GetLowestFScoreNode(List<Node> openList)
    {
        Node lowest = openList[0];
        foreach (Node node in openList)
        {
            if (node.FScore < lowest.FScore)
            {
                lowest = node;
            }
        }
        return lowest;
    }

    // Methode zur Ermittlung der benachbarten Knoten
    List<Node> GetNeighbors(Node current)
    {
        List<Node> neighbors = new List<Node>();
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = current.Position + dir;
            if (CanWalk(current.Position, neighborPos))
            {
                int cost = 1;
                if(current.Position.z-dir.z != 0) {
                    cost = Mathf.Abs(current.Position.z-dir.z) * 2;
                }
                neighbors.Add(new Node(neighborPos, cost));
            }
        }

        return neighbors;
    }

    // Methode zur Überprüfung, ob ein Knoten begehbar ist
    bool CanWalk(Vector3Int current, Vector3Int neighbor)
    {
        if(!mapBehaviour.hasVec(neighbor)) return false;
        return true;
    }
}
