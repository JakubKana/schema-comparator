namespace DBSchemaComparator.Domain.Models.SQLServer
{
   public interface IConstraint
    {
        string ConstraintName { get; set; }
        string ConstraintTable { get; set; }
    }
}
