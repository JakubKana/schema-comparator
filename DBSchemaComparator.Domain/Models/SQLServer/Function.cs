using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public class Function
    {
        [Column("FUNCTION_NAME")]
        public string Name { get; set; }
        [Column("FUNCTION_BODY")]
        public string Body { get; set; }
    }
}
