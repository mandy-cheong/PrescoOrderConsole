using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using goodmaji;
using PrescoOrderConsole.Modal;

/// <summary>
/// Summary description for ShipmentData
/// </summary>
public class ShipmentData
{
    public ShipmentData()
    {
        //
        // TODO: Add constructor logic here
        //
        ShipItems = new List<ShipItem>();
    }

    public Shipment Shipment { get; set; }

    public List<ShipItem> ShipItems { get; set; }

}