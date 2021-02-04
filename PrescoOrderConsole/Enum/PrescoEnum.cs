using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PrescoEnum
/// </summary>
public class PrescoEnum
{
    public PrescoEnum()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public enum PrescoAPI
    {
        Failed = -1,
        Sent = 1,
        NotSent = 0
    }

    public enum AccountType
    {
        GM,
        SF
    }

    public enum ShipmentType
    {
        ShopCollect = 0,
        Delivery = 1
    }
}