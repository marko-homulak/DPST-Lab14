using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab14
{
    public class FloydWarshall
    {
        private int[,] dist;
        private int?[,] next;
        private readonly int INF = int.MaxValue / 2;

        public int[,] Distances => dist;

        public FloydWarshall(int[,] adjacencyMatrix)
        {
            int n = adjacencyMatrix.GetLength(0);
            dist = new int[n, n];
            next = new int?[n, n];

            // Ініціалізація
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0;
                        next[i, j] = i;
                    }
                    else if (adjacencyMatrix[i, j] < INF)
                    {
                        dist[i, j] = adjacencyMatrix[i, j];
                        next[i, j] = j;
                    }
                    else
                    {
                        dist[i, j] = INF;
                        next[i, j] = null;
                    }
                }
            }

            FloydWarshallParallel(n);
        }

        private void RunFloydWarshall(int n)
        {
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }
        }

        private void FloydWarshallParallel(int n)
        {
            for (int k = 0; k < n; k++)
            {
                Parallel.For(0, n, i =>
                {
                    for (int j = 0; j < n; j++)
                    {
                        int ik = dist[i, k];
                        int kj = dist[k, j];
                        if (ik < INF && kj < INF && ik + kj < dist[i, j])
                        {
                            dist[i, j] = ik + kj;
                            next[i, j] = next[i, k];
                        }
                    }
                });
            }
        }

        public List<int> ReconstructPath(int start, int end)
        {
            if (next[start, end] == null)
                return new List<int>();

            List<int> path = new List<int> { start };
            while (start != end)
            {
                start = next[start, end].Value;
                path.Add(start);
            }

            return path;
        }

        public int GetDistance(int start, int end)
        {
            return dist[start, end];
        }

    }
}
