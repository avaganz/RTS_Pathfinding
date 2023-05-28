using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTS_AStar
{
    private class Node : IComparable<Node>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;
        public Node Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
            G = 0;
            H = 0;
            Parent = null;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }
    }

    private int[,] map;
    private int width;
    private int height;
    private Node[,] nodes;

    public RTS_AStar(int[,] map)
    {
        this.map = map;
        width = map.GetLength(0);
        height = map.GetLength(1);
        nodes = new Node[width, height];
        InitializeNodes();
    }

    private void InitializeNodes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new Node(x, y);
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        Node startNode = nodes[start.x, start.y];
        Node targetNode = nodes[target.x, target.y];

        if (GetNeighbors(startNode).Count() == 0)
            return null;

        PriorityQueue<Node> openSet = new PriorityQueue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Enqueue(startNode, startNode.F);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();

            if (currentNode == targetNode)
            {
                return ConstructPath(currentNode);
            }

            closedSet.Add(currentNode);

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGScore = currentNode.G + GetDistance(currentNode, neighbor);

                if (tentativeGScore < neighbor.G || !openSet.Contains(neighbor))
                {
                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeGScore;
                    neighbor.H = GetDistance(neighbor, targetNode);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, neighbor.F);
                    }
                }
            }
        }

        Node nearestNode = GetNearestFreeNode(targetNode, closedSet);
        if (nearestNode != null)
        {
            return ConstructPath(nearestNode);
        }

        return null;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int x = node.X;
        int y = node.Y;

        if (x > 0 && map[x - 1, y] == 0)
        {
            neighbors.Add(nodes[x - 1, y]);
        }
        if (x < width - 1 && map[x + 1, y] == 0)
        {
            neighbors.Add(nodes[x + 1, y]);
        }
        if (y > 0 && map[x, y - 1] == 0)
        {
            neighbors.Add(nodes[x, y - 1]);
        }
        if (y < height - 1 && map[x, y + 1] == 0)
        {
            neighbors.Add(nodes[x, y + 1]);
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dx = Math.Abs(nodeA.X - nodeB.X);
        int dy = Math.Abs(nodeA.Y - nodeB.Y);
        return dx + dy;
    }

    private List<Vector2Int> ConstructPath(Node node)
    {
        var path = new List<Vector2Int>();

        while (node != null)
        {
            path.Add(new Vector2Int(node.X, node.Y));
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }

    private Node GetNearestFreeNode(Node targetNode, HashSet<Node> nodes)
    {
        int maxDistance = Math.Max(width, height);
        for (int distance = 1; distance <= maxDistance; distance++)
        {
            for (int x = targetNode.X - distance; x <= targetNode.X + distance; x++)
            {
                for (int y = targetNode.Y - distance; y <= targetNode.Y + distance; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        Node node = nodes.FirstOrDefault(n => n.X == x && n.Y == y);
                        if (node != null)
                        {
                            return node;
                        }
                    }
                }
            }
        }

        return null;
    }
}

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> heap;

    public int Count => heap.Count;

    public PriorityQueue()
    {
        heap = new List<T>();
    }

    public void Enqueue(T item, int priority)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1, priority);
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty.");
        }

        T firstItem = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        HeapifyDown(0);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return heap.Contains(item);
    }

    private void HeapifyUp(int index, int priority)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[parentIndex].CompareTo(heap[index]) <= 0)
            {
                break;
            }
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int leftChild;
        int rightChild;
        int smallestChild;
        int lastIndex = heap.Count - 1;

        while (true)
        {
            leftChild = 2 * index + 1;
            rightChild = 2 * index + 2;
            smallestChild = index;

            if (leftChild <= lastIndex && heap[leftChild].CompareTo(heap[smallestChild]) < 0)
            {
                smallestChild = leftChild;
            }

            if (rightChild <= lastIndex && heap[rightChild].CompareTo(heap[smallestChild]) < 0)
            {
                smallestChild = rightChild;
            }

            if (smallestChild == index)
            {
                break;
            }

            Swap(index, smallestChild);
            index = smallestChild;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}
