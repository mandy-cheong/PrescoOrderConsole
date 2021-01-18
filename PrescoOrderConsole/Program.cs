using PrescoOrderConsole.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrescoOrderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var shipmentService = new ShipmentService();
            shipmentService.ProcessOrders();
        }
    }
}
