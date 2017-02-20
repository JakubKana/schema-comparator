namespace DBSchemaComparator.Domain.Models.Test
{
    public class TestResult
    {
        public ObjectType ObjectType { get; set; }
        public string TestedObjectName { get; set; }
        public ErrorTypes ErrorType { get; set; }
        public string Description { get; set; }
       
    }
}
