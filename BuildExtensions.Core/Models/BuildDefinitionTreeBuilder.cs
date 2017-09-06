using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildTree.Models
{
    public static class BuildDefinitionTreeBuilder
    {
        public static IList<BuildDefinitionTreeNode> Build(IEnumerable<IBuildDefinition> buildDefinitions, char splitChar)
        {
            var root = new BuildDefinitionTreeNode("root");
            foreach (var buildDefinition in buildDefinitions)
            {
                var name = buildDefinition.Name;
                root = BuildTree(root, buildDefinition, name.Split(splitChar));
            }

            // Now that the tree is built, sort each node
            SortNode(root);
            return root.Children;
        }

        private static BuildDefinitionTreeNode BuildTree(BuildDefinitionTreeNode node, IBuildDefinition model, string[] tail)
        {
            if (tail.Any())
            {
                var head = tail.First();

                if (node.Children.Any(n => n.Name.Equals(head, StringComparison.OrdinalIgnoreCase)))
                {
                    BuildTree(node.Children.Single(n => n.Name.Equals(head, StringComparison.OrdinalIgnoreCase)), model, tail.Skip(1).ToArray());
                }
                else
                {
                    node.Children.Add(BuildTree(new BuildDefinitionTreeNode(head), model, tail.Skip(1).ToArray()));
                }
            }
            else
            {
                node.BuildDefinition = model;
            }

            return node;
        }

        private static void SortNode(BuildDefinitionTreeNode node)
        {
            // push leaf nodes to the bottom, then sort by name
            node.Children.Sort((x, y) =>
                               {
                                   var anyX = x.Children.Any();
                                   var anyY = y.Children.Any();
                                   return anyX != anyY
                                       ? (anyX ? -1 : 1)
                                       : String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
                               });
            node.Children.ForEach(SortNode);
        }
 
    }

}
