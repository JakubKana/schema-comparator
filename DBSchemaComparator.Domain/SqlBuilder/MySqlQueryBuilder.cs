﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    public class MySqlQueryBuilder : SqlBaseBuilder
    {


        protected override string GetTableString()
        {
            return @"";
        }
    }
}
