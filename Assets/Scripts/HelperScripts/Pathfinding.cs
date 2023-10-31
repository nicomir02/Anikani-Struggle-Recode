using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;

public class Pathfinding
{

    private class Node {
        public Vector3Int position;
        public Node parent;
        public int gCost;
        public int hCost;

        public int FCost { get { return gCost + hCost; } }
    }

    public Vector3Int start;
    public Vector3Int target;
    public List<Vector3Int> Path;
    Unit u;

    private MapBehaviour mapBehaviour;
    private TilemapHover tilemapHover;

    public int endkosten = -1;



    public Pathfinding(Vector3Int StartPoint, Vector3Int EndPoint, Unit unit){
        start = StartPoint;
        target = EndPoint;
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        tilemapHover = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        u = unit;
    }

   

    public List<Vector3Int> FindPath(){
        Vector3Int currentPoint = start;
       
        

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        Node startNode = new Node { position = start };
        Node targetNode = new Node { position = target };

        openList.Add(startNode);
        Debug.Log("[Pathfinding] Try to find Path from " + startNode.position + " to " + targetNode.position +".");

        while (openList.Count > 0) {
            Node currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].FCost < currentNode.FCost || 
                    (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost)) {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                Debug.Log("[Pathfinding] Path found");
                // Pfad gefunden, rekonstruieren Sie den Pfad
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector3Int neighborPosition in getNeighborTiles(currentNode.position))
            {
                if (!CanWalk(neighborPosition) || closedList.Any(n => n.position == neighborPosition))
                    continue;

                int moveCostToNeighbor = currentNode.gCost + 1;

        
                if(Mathf.Abs(mapBehaviour.getZ(currentNode.position) - mapBehaviour.getZ(neighborPosition)) > 1) {     //wenn mehr als ein Höhenunterschied ist dann soll es nicht möglich sein
                    moveCostToNeighbor += 1000;
                }else if(Mathf.Abs(mapBehaviour.getZ(currentNode.position) - mapBehaviour.getZ(neighborPosition)) == 1) {
                    moveCostToNeighbor++;     //kostet mehr wenn man hoch oder runter geht auf z Höhe
                }  

                Debug.Log(Mathf.Abs(mapBehaviour.getZ(currentNode.position)) + " " + Mathf.Abs(mapBehaviour.getZ(neighborPosition)));
                
                Node neighborNode = new Node
                {
                    position = neighborPosition,
                    parent = currentNode,
                    gCost = moveCostToNeighbor,
                    hCost = GetDistance(neighborPosition, target)
                };

                if (!openList.Contains(neighborNode))
                {
                    openList.Add(neighborNode);
                }
                
            }
        }
        Debug.Log("[Pathfinding] No path found.");
        // Kein Pfad gefunden
        return null;
    }

    public int GetDistance(Vector3Int pointA, Vector3Int pointB) {
        int dx = Mathf.Abs(pointA.x - pointB.x);
        int dy = Mathf.Abs(pointA.y - pointB.y);

        int dz = Mathf.Abs(mapBehaviour.getZ(pointA) - mapBehaviour.getZ(pointB));
        return dx + dy + dz;
    }

    private List<Vector3Int> RetracePath(Node startNode, Node endNode) {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        endkosten = endNode.gCost;

        //if(endkosten == 0 && startNode.position != endNode.position) endkosten = 1;
        
        if(endkosten > u.Steps) {
            return null;
        }

        
        
        while (currentNode.position != startNode.position)
        {
            Debug.Log(currentNode);
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
            Debug.Log("Parent: " + currentNode);
            Debug.Log("StartNode:" + startNode);
        }

        // Der Pfad ist derzeit rückwärts, also müssen wir ihn umdrehen
        path.Reverse();

        return path;
    }

    private List<Vector3Int> getNeighborTiles(Vector3Int currentPoint){
        List<Vector3Int> neighborTiles = new List<Vector3Int>();
        Vector3Int temp = currentPoint;
        temp.x += 1;
        temp.z = mapBehaviour.getZ(temp);
        neighborTiles.Add(temp);
        temp = currentPoint;
        temp.x -= 1;
        temp.z = mapBehaviour.getZ(temp);
        neighborTiles.Add(temp);
        temp = currentPoint;
        temp.y += 1;
        temp.z = mapBehaviour.getZ(temp);
        neighborTiles.Add(temp);
        temp = currentPoint;
        temp.y -= 1;
        temp.z = mapBehaviour.getZ(temp);
        neighborTiles.Add(temp);
        return neighborTiles;
    }

    private bool CanWalk(Vector3Int vec){
        return true;
    }















    /*
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
    Unit u;

    private int allCost = -1;

    private MapBehaviour mapBehaviour;

    public Pathfinding(Vector3Int StartPoint, Vector3Int EndPoint, Unit unit)
    {
        start = StartPoint;
        target = EndPoint;
        mapBehaviour = GameObject.Find("___InGame___/----Manager----/MapManager").GetComponent<MapBehaviour>();
        u = unit;
    }

    public int getAllCost() {
        return allCost;
    }

    public List<Vector3Int> resultListe(List<Vector3Int> liste) {
        start.z = 0;
        List<Vector3Int> resultliste = new List<Vector3Int>();
        foreach(Vector3Int vec in liste) {
            Vector3Int v = new Vector3Int(vec.x, vec.y, 0);
            if(!resultliste.Contains(v) && start != v) {
                resultliste.Add(v);
                Debug.Log(v);
            }
        }
        return resultliste;
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
                return resultListe(ReconstructPath(current));
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

    */
}
