using System.Collections.Generic;

namespace DBSchemaComparator.Domain.Models.Test
{
    public class TestNodes
    {
        public ObjectType NodeType { get; set; }
        public string Description { get; set; }
        public List<TestNodes> Nodes { get; set; }
        public List<TestResult> Results { get; set; }
        public ResultLevel ResultLevel { get; set; }

    }
}
