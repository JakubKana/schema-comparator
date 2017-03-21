using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    [TableName("VIEWS")]
    public class View : IProcedure
    {
        [Column("VIEW_NAME")]
        public string Name { get; set; }
        [Column("VIEW_BODY")]
        public string Body { get; set; }
    }
}
