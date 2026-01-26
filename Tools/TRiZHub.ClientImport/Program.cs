using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRiZHub.BL.Context;
using TRiZHub.ClientImport.ImportEngine;

namespace TRiZHub.ClientImport
{
    class Program
    {
        public static void Main(string[] args)
        {
            ClientImportEngine.Instance().ImportClientData(new DataContext());
        }
    }
}
