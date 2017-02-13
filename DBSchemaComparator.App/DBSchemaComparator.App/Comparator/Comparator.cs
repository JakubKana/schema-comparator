using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Test;

namespace DBSchemaComparator.App.Comparator
{
   public class Comparator
    {
        private string ConnStringLeft { get; set; }
        private string ConnStringRight { get; set; }

        private List<TestResult> _testResults = new List<TestResult>();

        public Comparator(string connStringLeft, string connStringRight)
        {
            ConnStringLeft = connStringLeft;
            ConnStringRight = connStringRight;
        }


        public bool ConnectToDatabase(string connectionString)
        {
            return true;


        }

    }
}
