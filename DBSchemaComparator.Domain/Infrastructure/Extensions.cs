using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.Domain.Infrastructure
{
   public static class Extensions
    {
        
        public static IEnumerable<T> DepthFirstTraversal<T>(T start,Func<T, IEnumerable<T>> getNeighbors)
        {
            var visited = new HashSet<T>();
            var stack = new Stack<T>();
            stack.Push(start);

            while (stack.Count != 0)
            {
                var current = stack.Pop();
                visited.Add(current);
                yield return current;

                var neighbors = getNeighbors(current).Where(node => !visited.Contains(node));

                foreach (var neighbor in neighbors.Reverse())
                {
                    stack.Push(neighbor);
                }
            }
        }
    
    }
}
