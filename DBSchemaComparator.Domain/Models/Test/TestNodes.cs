using System.Collections.Generic;

namespace DBSchemaComparator.Domain.Models.Test
{
    public class TestNodes
    {
        public TestNodeType TestType { get; set; }
        public string TestNodeName { get; set; }
        public List<TestNodes> Nodes { get; set; }
        public List<TestResult> Results { get; set; }

    }
}
