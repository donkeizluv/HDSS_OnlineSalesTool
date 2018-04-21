using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Config
{
    public class Config
    {
        public string ConnectionStrings { get; set; }
        /// <summary>
        /// Default sync interval for no trigger defined
        /// </summary>
        public int SyncInterval { get; set; }
    }
}
