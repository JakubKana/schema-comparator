using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace DBSchemaComparator.Domain.Infrastructure
{
   public static class Extensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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

        public static IEnumerable<T> RemoveDuplicity<T>(IEnumerable<T> list)
        {
            Logger.Info("Removing duplicities from the list.");
            var distinctList = list.Distinct();
            Logger.Debug("Returning distinct list", distinctList.ToString());
            return distinctList;
        }

        public static string Normalize(string text)
        {
            Logger.Info("Running Normalize method.");
            Logger.Debug($"Normalizing text:\n {text}");
            string pattern = @"(\s+)";

            var normalizedText = Regex.Replace(text, pattern, " ");

            Logger.Debug($"Normalized text:\n {normalizedText}");
            return normalizedText;
        }

        public static string RemoveBeginingNewLine(string text)
        {
            Logger.Info("Running RemoveBeginingNewLine method.");
            Logger.Debug($"Normalizing text:\n {text}");
            string pattern = @"(^\n+)";

            var normalizedText = Regex.Replace(text, pattern, "");

            Logger.Debug($"Normalized text:\n {normalizedText}");
            return normalizedText;
        }
        public static string NormalizeParameters(string text)
        {
            Logger.Info("Running replace");
            Logger.Debug($"Replacing @ within string: {text}");
            var normalizeText = text.Replace("@", "@@");
            Logger.Debug($"Returning normalized string: {normalizeText}");
            return normalizeText;
        }

    }
}
