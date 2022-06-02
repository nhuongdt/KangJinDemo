using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace banhang24.Hellper
{
    public class BarCodeHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["vCode"] != null)
            {
                string _code = context.Request["vCode"].ToString();
                int _ht = context.Request["ht"] != null ? int.Parse(context.Request["ht"].ToString()) : 24;
                Barcode128 _bc = new Barcode128();
                _bc.CodeType = Barcode128.CODE128;
                _bc.ChecksumText = true;
                _bc.GenerateChecksum = true;
                _bc.StartStopText = true;
                _bc.BarHeight = _ht;
                _bc.X = 0.5f;
                _bc.Code = _code;
                _bc.Size = 5f;
                Bitmap _bm = new Bitmap(_bc.CreateDrawingImage(Color.Black, Color.White));
                Graphics _gr = Graphics.FromImage(_bm);
                _gr.PageUnit = GraphicsUnit.Pixel;
                _gr.Clear(Color.White);
                _gr.DrawImage(_bc.CreateDrawingImage(Color.Black, System.Drawing.Color.White), new Point(0, 0));
                _bm.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            }

        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}