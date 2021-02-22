using goodmaji;
using Newtonsoft.Json;
using PrescoOrderConsole.Logger;
using PrescoOrderConsole.Modal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static PrescoEnum;

/// <summary>
/// Summary description for PrescoService
/// </summary>
public class PrescoService
{
    //測試
    // public string _url = "https://test-cbec.sp88.tw";
    //正式
    // public string _url = "https://cbec.sp88.tw";
    private readonly APIHelper _apiHelper;

    public string _url = System.Configuration.ConfigurationManager.AppSettings["prescourl"];
    public PrescoService()
    {
        //
        // TODO: Add constructor logic here
        //
        _apiHelper = new APIHelper();
    }


    public RVal CreateOrder(List<OrderRequest> requests)
    {
        CheckRemainingShipNumber();
        var shipnumner = GetShipmentNumber(requests.Count);
        for (int i = 0; i < requests.Count; i++)
            requests[i].ShipNo = shipnumner[i].Number;

        var rval = SubmitOrder(requests); ;
        if (rval.RStatus)
        {
            UpdateShipNumber(shipnumner);
        }


        return rval;
    }

    private RVal RequestShipNumber(ShipNumberRequest request)
    {
        var rval = new RVal();
        try
        {

            var url = _url + "/api/shipment/numbers?CountryId=" + request.CountryId + "&ShipCount=" + request.ShipCount;
            var data = JsonConvert.SerializeObject(request);
            var helper = new APIHelper { Url = url, RequestData = data };
            rval = helper.GETApi();
            if (rval.RStatus)
            {
                rval.DVal = JsonConvert.DeserializeObject<List<string>>(rval.RMsg);
            }

            return rval;
        }
        catch (Exception ex)
        {
            Logger.AddLog(rval.RMsg, ex.Message);
        }
        return rval;
    }

    private  PrescoAPILog MapAPILog(APIHelper helper)
    {
        return new PrescoAPILog
        {
            SysId = Guid.NewGuid(),
            CDate = DateTime.Now,
            URL = helper.Url,
            RequestData = helper.RequestData,
            ResponseData = helper.ResponseData
        };
    }

    private RVal SubmitOrder(List<OrderRequest> request)
    {

        var url = _url + "/api/shipment/new";

        var data = JsonConvert.SerializeObject(request);
        var isDelivery = string.IsNullOrEmpty(request.FirstOrDefault().Collection);
        var tokenaccount = isDelivery ? PrescoEnum.AccountType.SF : PrescoEnum.AccountType.GM;
        var helper = new APIHelper { Url = url, RequestData = data, ContentType = "application/json", TokenAccount = tokenaccount };


        var rval = new RVal();
        try
        {
            rval = helper.PostApi();
            if (rval.RStatus == false)
                rval.RMsg = JsonConvert.DeserializeObject<PrescoResponse>(rval.RMsg).Message;

            AddOrderLog(request, helper, rval);
        }
        catch (Exception ex)
        {
            Logger.AddLog(rval.RMsg, ex.Message);
        }

        return rval;
    }

    private int AddOrderLog(List<OrderRequest> request, APIHelper aPIHelper, RVal rVal)
    {
        var cmdList = new List<SqlCommand>();
        var prescoAPILog = MapAPILog(aPIHelper);
        foreach (var item in request)
        {
            var orderlog = CreatePrescoLog(rVal, prescoAPILog, item);
            cmdList.Add(CreatePrescoLogCmd(orderlog));

            if (rVal.RStatus)
            {
                cmdList.Add(UpdateShipmenntCmd(orderlog));
                var shipment = new ShipmentStHistory
                {
                    SSH04 = item.PageCount,
                    SSH05 = orderlog.Status == (int)PrescoEnum.PrescoAPI.Failed ? (int)ShipmentStatus.OnHold : (int)ShipmentStatus.OrderReceived,
                    SSH02 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    SSH03 = item.OrderNo,
                    SSH27 = orderlog.Status == (int)PrescoEnum.PrescoAPI.Failed ? ShipDescription.API傳送失敗.ToString() : ""
                };
                cmdList.Add(SqlExtension.GetInsertSqlCmd("ShipmentStHistory", shipment));
            }
        }
        cmdList.Add(SqlExtension.GetInsertSqlCmd("PrescoAPILog", prescoAPILog));
        var rval = SqlDbmanager.ExecuteNonQryMutiSqlCmd(cmdList);
        return rval;
    }

    private SqlCommand CreatePrescoLogCmd(PrescoOrderLog orderlog)
    {
        var insertLogCmd = SqlExtension.GetInsertSqlCmd("Prescoorderlog", orderlog);
        insertLogCmd.CommandText = @"IF NOT EXISTS(SELECT TOP 1 GMSHIPID FROM Prescoorderlog WHERE GMSHIPID = @GMSHIPID )
                        BEGIN 
                        INSERT INTO Prescoorderlog(SysId, PrescoAPILogId, PrescoShipId , GMShipId , Msg , Status , HeaderMsg)
                        VALUES (@SysId , @PrescoAPILogId , @PrescoShipID , @GMShipId , @Msg , @Status , @HeaderMsg) 
                        END
                        ELSE 
                        BEGIN
                        UPDATE Prescoorderlog SET Status  = @Status , 
                        PrescoAPILogId = @PrescoAPILogId,
                        PrescoShipID = @PrescoShipID,
                        Msg = @Msg , 
                        HeaderMSg = @HeaderMsg 
                        WHERE GMSHIPID = @GMSHIPID 
                        END ";
        return insertLogCmd;
    }

    private static PrescoOrderLog CreatePrescoLog(RVal rVal, PrescoAPILog prescoAPILog, OrderRequest item)
    {
        var orderlog = new PrescoOrderLog();
        orderlog.SysId = Guid.NewGuid();
        orderlog.PrescoAPILogID = prescoAPILog.SysId;
        orderlog.PrescoShipID = rVal.RStatus ? item.ShipNo : "";
        orderlog.GMShipID = item.OrderNo;
        orderlog.Msg = rVal.RMsg;
        orderlog.Status = rVal.RStatus ? 1 : -1;
        orderlog.HeaderMsg = rVal.DVal == null ? "" : rVal.DVal;
        return orderlog;
    }

    private SqlCommand UpdateShipmenntCmd(PrescoOrderLog log)
    {
        var cmd = new SqlCommand();
        cmd.CommandText = "Update Shipment set ST69 = @PrescoShipID, ST12=@status where ST02 = @GMShipID";
        var shipmentStatus = log.Status == (int)PrescoEnum.PrescoAPI.Sent ? (int)ShipmentStatus.OrderReceived : (int)ShipmentStatus.OnHold;
        cmd.Parameters.Add(SafeSQL.CreateInputParam("@status", SqlDbType.Int, shipmentStatus));
        cmd.Parameters.Add(SafeSQL.CreateInputParam("@PrescoShipID", SqlDbType.NVarChar, log.PrescoShipID));
        cmd.Parameters.Add(SafeSQL.CreateInputParam("@GMShipID", SqlDbType.NVarChar, log.GMShipID));
        return cmd;
    }
    private bool AddLog(APIHelper helper)
    {
        PrescoAPILog prescoAPILog = MapAPILog(helper);
        var cmd = SqlExtension.GetInsertSqlCmd("PrescoAPILog", prescoAPILog);
        return SqlDbmanager.ExecuteNonQry(cmd);
    }

    public void CheckRemainingShipNumber()
    {
        if (CountShipmentNumber() <= 10)
        {
            var requestShipNumber = RequestShipNumber(new ShipNumberRequest { ShipCount = 100, CountryId = "HK" }).DVal;
            var addNumbers = MapShipNumber(requestShipNumber);
            AddShipNumber(addNumbers);
        }
    }
    private int AddShipNumber(List<ShipmentNumber> shipmentNumbers)
    {
        var cmdList = new List<SqlCommand>();
        foreach (var number in shipmentNumbers)
        {
            cmdList.Add(SqlExtension.GetInsertSqlCmd("PrescoShipment", number));

        }
        var rval = SqlDbmanager.ExecuteNonQryMutiSqlCmd(cmdList);
        return rval;
    }
    private int UpdateShipNumber(List<ShipmentNumber> shipmentNumbers)
    {
        var cmdlist = new List<SqlCommand>();
        foreach (var shipnumber in shipmentNumbers)
        {
            shipnumber.Status = (int)ShipNumberStatus.Used;
            shipnumber.UDate = DateTime.Now;
            cmdlist.Add( SqlExtension.GetUpdateSqlCmd("PrescoShipment", shipnumber, new List<string> { "Id" }, "Id=@Id"));
        }
        var rval = SqlDbmanager.ExecuteNonQryMutiSqlCmd(cmdlist);

        return rval;
    }

    private int CountShipmentNumber()
    {
        var sql = "SELECT  Count(1) FROM PrescoShipment WHERE Status =1  ";
        var rval = int.Parse(SqlDbmanager.ExecuteScalar(sql).ToString());
        return rval;

    }
    private List<ShipmentNumber> GetShipmentNumber(int topcount)
    {

        var cmd = new SqlCommand { CommandText = "SELECT TOP " + topcount + " * FROM PrescoShipment WHERE Status =1 ORDER BY CDate " };
        var dt = SqlDbmanager.queryBySql(cmd);
        var result = new List<ShipmentNumber>();
        foreach (DataRow dr in dt.Rows)
        {
            var number = new ShipmentNumber();
            number.Id = int.Parse(dr["id"].ToString());
            number.Number = dr["Number"].ToString();
            result.Add(number);
        }
        return result;

    }
    private List<ShipmentNumber> MapShipNumber(List<string> numbers)
    {
        var shipnumbers = new List<ShipmentNumber>();
        foreach (var num in numbers)
        {
            shipnumbers.Add(new ShipmentNumber
            {
                Number = num,
                CDate = DateTime.Now,
                Status = (int)ShipNumberStatus.Active
            });
        }
        return shipnumbers;
    }
   
}


