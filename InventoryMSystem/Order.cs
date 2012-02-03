using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory
{
    public class Order
    {
        public string productName;
        public int quantity;
        public string state;
        public Order()
        {
            this.productName = string.Empty;
            this.quantity = 0;
        }
        public Order(string _productName, int _quantity)
        {
            this.productName = _productName;
            this.quantity = _quantity;
        }
    }
}
