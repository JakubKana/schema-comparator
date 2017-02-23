using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{ 
    [TableName("ROUTINES")]
    public class StoredProcedure : IProcedure
    {
        [Column("PROCEDURE_NAME")]
        public string Name { get; set; }
        [Column("PROCEDURE_BODY")]
        public string Body { get; set; }

    }
}
