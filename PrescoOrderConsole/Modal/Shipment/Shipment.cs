using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrescoOrderConsole.Modal
{
  public  class Shipment
    {
        public Guid ST01 { get; set; }
        public String ST02 { get; set; }
     
        public int? ST05 { get; set; }
        public Decimal? ST06 { get; set; } 
        public Decimal? ST07 { get; set; } 
        public String ST08 { get; set; }
        public String ST09 { get; set; }
        public int? ST10 { get; set; }
        public String ST11 { get; set; } 
        public int? ST12 { get; set; } 
        public String ST13 { get; set; } 
        public int? ST14 { get; set; } 
        public String ST15 { get; set; } 
        public int? ST16 { get; set; } 
        public String ST17 { get; set; } 
        public String ST18 { get; set; } 
        public String ST19 { get; set; } 
        public String ST20 { get; set; } 
        public String ST21 { get; set; }
        public String ST22 { get; set; }

        public String ST23 { get; set; }
        public String ST24 { get; set; }
        public String ST25 { get; set; }
        public String ST26 { get; set; }
        public String ST27 { get; set; }
        public String ST28 { get; set; }
        public String ST29 { get; set; }
        public String ST30 { get; set; }
        public String ST31 { get; set; }
        public String ST32 { get; set; }
        public String ST33 { get; set; }
        public String ST34 { get; set; }
        public Decimal? ST35 { get; set; }
        public Decimal? ST36 { get; set; }
        public Decimal? ST37 { get; set; }
        public Decimal? ST38 { get; set; }
        public String ST39 { get; set; }
        public String ST40 { get; set; }
        public string ST41 { get; set; }
        public string ST42 { get; set; } 

        public string ST43 { get; set; }

        public string ST44 { get; set; }

        public Guid ST45 { get; set; }
        public Decimal? ST46 { get; set; } 
        public String ST47 { get; set; }
        public int? ST48 { get; set; }
        public int? ST49 { get; set; }
        public String ST50 { get; set; }
        public String ST51 { get; set; }
        public string ST52 { get; set; }
        public string ST53 { get; set; }
        public int? ST54 { get; set; } 
        public int? ST55 { get; set; }
        public string ST56 { get; set; }
        public string ST57 { get; set; }
        public string ST58 { get; set; }
        public string ST59 { get; set; }
        public string ST60 { get; set; }
        public int? ST61 { get; set; }
        public string ST62 { get; set; }
        public string ST63 { get; set; }
        public string ST64 { get; set; }
        public string ST65 { get; set; }
        public string ST66 { get; set; }
        public string ST67 { get; set; }
        public string ST68 { get; set; }
        public string ST69 { get; set; }
        public string ST70 { get; set; }
        public string ST71 { get; set; }
        public Decimal? ST72 { get; set; } 
        public int? ST73 { get; set; }
        public string ST74 { get; set; }
        public string ST75 { get; set; }
        public int? ST76 { get; set; }
        public int? ST77 { get; set; }
        public string ST78 { get; set; }
        public int? ST79 { get; set; }
        public int? ST80 { get; set; }
        public int? ST81 { get; set; }
        public int? ST82 { get; set; }
        public string ST83 { get; set; }
        public string ST84 { get; set; }
        public int? ST85 { get; set; }
        public string ST86 { get; set; }
        public string ST87 { get; set; }
        public string ST88 { get; set; }
        public int? ST89 { get; set; }
        public string ST90 { get; set; }
        public string ST91 { get; set; }
        public int? ST92 { get; set; }
        public int? ST93 { get; set; }

        public string ST94 { get; set; }

        public Guid PrescoLogId { get; set; }

    }

    public enum ShipmentStatus
    {
        OnHold = -6,
        Close = -5,
        ReturnedToHub = -4,
        DepartFromTPE = -3,
        Cancel = -2,
        Pending = -1,
        OrderReceived = 0,
        ShipmentReceived = 1,
        OutboundProcess = 2,
        Outbind = 3,
        CustomCleareance = 4,
        InTransit = 5,
        DeliveryCompleted = 6,
        ArrivedAtDistributionPoint = 7
    }

    public enum Clearance
    {
        不報關 = -1,
        簡易報關 = 0,
        正式報關 = 1,
        合併報關 = 2,
        合併簡報 = 3,
        合併正報 = 4,
    }

    public enum Service
    {

        一般 = 0,
        經濟 = 1,
        快遞 = 2,
        運費到付 = 3,
        其他 = 4,
        馬來西亞倉 = 5,
        台灣倉 = 6,
        海運 = 7,
        尚未設定 = 8,
        COD手續費 = 9,
        代收手續費 = 10,
        出口銷售 = 11,
        賠償費用 = 12,
        折讓單 = 13,
        運費補款 = 14,
        店取 = 15,

    }

    public enum ShipDescription
    {
        API尚未建立 = 3,
        API傳送失敗 = 4

    }
}

