using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.Domain.Models.General
{
    [Flags]
    public enum ExitCodes : int
    {
        Success = 0,
        InvalidArguments = 1,
        ScriptFailed = 5,
        UnsupportedDbType = 6
    }
}
