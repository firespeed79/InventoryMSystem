using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory
{
    public class ProductName
    {
        public string name;
        public ProductName()
        {
            name = string.Empty;
        }
        public ProductName(string n)
        {
            this.name = n;
        }
    }
}
