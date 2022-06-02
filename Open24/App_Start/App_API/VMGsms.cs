using Open24.SMSVMGAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open24.App_Start.App_API 
{
    public class VMGsms
    {
        public static int SendMsg(string sdt, string activecode)
        {
            string authenticateUser = "ssoftvn1", authenticatePass = "vmg123456";
            sdt = "84" + sdt.Remove(0, 1);
            VMGAPISoapClient sms = new VMGAPISoapClient();
            ApiBulkReturn apiresult = sms.BulkSendSms(sdt, "Ssoftvn", "Ma kich hoat phan mem Open24.vn: " + activecode, "", authenticateUser, authenticatePass);
            return apiresult.error_code;
        }
    }
}