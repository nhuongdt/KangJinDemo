using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace KetNoiDuocQuocGia
{
    public class Config
    {
        public string linkDuocQuocGia = "";
        public string apiDangNhap = "api/tai_khoan/dang_nhap";
        public string apiDonThuoc = "/api/lien_thong/don_thuoc";
        public string apiHoaDon = "/api/lien_thong/hoa_don";
        public string apiPhieuNhap = "/api/lien_thong /phieu_nhap";
        public string apiPhieuXuat = "/api/lien_thong/phieu_xuat";
        public string apiThemMoiThuocCoSo = "/api/lien_thong/thuoc_co_so/them_thuoc";
        public string apiCapNhatThuocCoSo = "/api/lien_thong/thuoc_co_so/cap_nhat_thuoc";
        public string apiXemThuocCoSo = "/api/lien_thong/thuoc_co_so/xem_thuoc";
        public string apiXoaThuocCoSo = "/api/lien_thong/thuoc_co_so/xoa_thuoc";
        public string contentType = "application/json";

        public async System.Threading.Tasks.Task<string> GetTokenAsync(UserDangNhap obj)
        {
            using (var httpClient = new HttpClient())
            {
                string uri = linkDuocQuocGia + apiDangNhap;
                string jsondata = JsonConvert.SerializeObject(obj);
                string tokenResult = "";
                HttpContent content = new StringContent(jsondata, UTF8Encoding.UTF8, contentType);
                var response = await httpClient.PostAsync(uri, content);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TokenObject tokenObject = JsonConvert.DeserializeObject<TokenObject>(response.Content.ToString());
                    tokenResult = tokenObject.token;
                }
                return tokenResult;
            }    
        }
    }
}
