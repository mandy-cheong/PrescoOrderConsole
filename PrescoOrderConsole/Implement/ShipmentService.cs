using goodmaji;
using PrescoOrderConsole.Modal;
using PrescoOrderConsole.Modal.Presco;
using SqlLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrescoOrderConsole.Implement
{
   public class ShipmentService
    {
        private readonly DapperHelper _dapperHelper;
        private readonly PrescoService _prescoService;
        public ShipmentService()
        {
            _dapperHelper = new DapperHelper();
            _prescoService = new PrescoService();
        }
    
        public void ProcessOrders()
        {
            var onHoldShipmentData = GetOnHoldShipmentData();

            foreach (var shipmentData in onHoldShipmentData)
            {
                var shipmentdatas = new List<ShipmentData>();
                shipmentdatas.Add(shipmentData);
                var prescoOrder = MapOrderRequest(shipmentdatas);

                var rval = new RVal { RStatus = true };
                var result = _prescoService.CreateOrder(prescoOrder);
            }
        }
        public List<ShipmentData> GetOnHoldShipmentData()
        {
            var onHoldShipmetnDatas = new List<ShipmentData>();
            var onHoldShipments = GetOnHoldShipment();
            var onHoldShipItems = GetOnHoldShipItem();

            foreach (var onHoldShipment in onHoldShipments)
            {
                var shipment = new ShipmentData();
                var shipItem = onHoldShipItems.Where(x => x.SI16 == onHoldShipment.ST01).ToList();
             
                if (onHoldShipment != null)
                    shipment.ShipItems = shipItem;

                shipment.Shipment = onHoldShipment;
                onHoldShipmetnDatas.Add(shipment);
            }

            return onHoldShipmetnDatas;
        }

        public List<Shipment> GetOnHoldShipment()
        {
            var sql = "SELECT * FROM Shipment WHERE ST12=@OnHoldStatus AND ST28=@COUNTRY AND (ST84!='' OR ST84 IS NOT NULL) ";
            return _dapperHelper.Query<Shipment>(sql, new { @OnHoldStatus = (int)ShipmentStatus.OnHold, @Country = "HK" }).ToList();

        }
        public List<ShipItem> GetOnHoldShipItem()
        {
            var sql = "SELECT ShipItem.* FROM ShipItem INNER JOIN Shipment ON Shipitem.SI16= Shipment.ST01 WHERE ST12=@OnHoldStatus AND ST28=@COUNTRY AND (ST84!='' OR ST84 IS NOT NULL) ";
            return _dapperHelper.Query<ShipItem>(sql, new { @OnHoldStatus = (int)ShipmentStatus.OnHold, @Country = "HK" }).ToList();

        }

        public void UpdatePrescoOrder()
        { 
        }

        private List<OrderRequest> MapOrderRequest(List<ShipmentData> shipmentDatas)
        {
            var orders = new List<OrderRequest>();
            foreach (var data in shipmentDatas)
            {
                var order = new OrderRequest();

                order.PageCount = data.ShipItems.Count;
                order.OrderNo = data.Shipment.ST02;
                order.Collection = data.Shipment.ST84;
                order.ReceiverCountry = data.Shipment.ST28;
                order.ReceiverName = data.Shipment.ST25;
                order.ReceiverTel1 = data.Shipment.ST26;
                order.ReceiverTel2 = "";
                order.ReceiverArea = data.Shipment.ST29;
                order.ReceiverCode = data.Shipment.ST52;
                order.ReceiverAddr = data.Shipment.ST30;
                order.ReceiverEmail = data.Shipment.ST50;
                order.Weight = data.Shipment.ST06.HasValue ? data.Shipment.ST06.Value : 0;
                order.Height = data.Shipment.ST37.HasValue ? data.Shipment.ST37.Value : 0;
                order.Width = data.Shipment.ST35.HasValue ? data.Shipment.ST35.Value : 0;
                order.Length = data.Shipment.ST36.HasValue ? data.Shipment.ST36.Value : 0;
                order.SenderName = data.Shipment.ST17;
                order.SenderAddr = data.Shipment.ST23;
                order.SenderTel1 = data.Shipment.ST18;
                order.SenderTel2 = "";
                order.SenderCountry = data.Shipment.ST21;
                order.SenderEmail = data.Shipment.ST51;
                order.SenderArea = data.Shipment.ST22;
                order.SenderCode = data.Shipment.ST53;
                order.Clearance = data.Shipment.ST05.HasValue? Clearance.不報關.ToString():(Enum.Parse(typeof(Clearance), data.Shipment.ST05.Value.ToString())).ToString(); // clearance.SelectedValue;
                order.Type = "";
                order.Service = "";
                order.Pay =   data.Shipment.ST54.HasValue ? Service.一般.ToString() : (Enum.Parse(typeof(Service), data.Shipment.ST54.Value.ToString())).ToString(); // clearance.SelectedValue;
                order.Note = data.Shipment.ST33;
                order.CodCurrency = data.Shipment.ST46.ToString();
                order.Currency = data.Shipment.ST43;
                order.DeliveryType = "店取";
                order.RecipentDate = "";
                order.ParentId = "";
                order.EshopId = "";

                foreach (var shipitem in data.ShipItems)
                {
                    var package = new Package();
                    package.Qty = shipitem.SI04.HasValue ? shipitem.SI04.Value : 0;
                    package.Price = shipitem.SI05.HasValue ? shipitem.SI05.Value : 0;
                    package.EnglishName = shipitem.SI03;
                    package.ChineseName = shipitem.SI12;
                    package.Brand = shipitem.SI13;
                    package.NetWeight = shipitem.SI10.HasValue ? shipitem.SI10.Value : 0;
                    order.Package.Add(package);
                }
                orders.Add(order);
            }

            return orders;
        }

    }
}
