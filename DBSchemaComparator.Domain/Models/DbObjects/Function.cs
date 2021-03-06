﻿using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    [TableName("ROUTINES")]
    public class Function : IProcedure
    {
        [Column("FUNCTION_NAME")]
        public string Name { get; set; }
        [Column("FUNCTION_BODY")]
        public string Body { get; set; }
    }
}
