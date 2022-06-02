using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class QuocGiaInitializer
    {
        #region
        public static Guid idQGia_VNi = Guid.NewGuid();
        public static List<DM_QuocGia> lstQGiaInitializer = new List<DM_QuocGia>()
        {
            new DM_QuocGia{ID=idQGia_VNi ,MaQuocGia="VNI",  GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn",TenQuocGia="Việt Nam"}
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Afghanistan",TenQuocGia="Afghanistan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Egypt",TenQuocGia="Ai cập",  GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Albania",TenQuocGia="Albania", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Algeria",TenQuocGia="Algérie", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Andorra",TenQuocGia=" Andorra", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Angola",TenQuocGia="Angola", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Austria",TenQuocGia="Áo", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Argentina",TenQuocGia="Argentina", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() ,MaQuocGia="Armenia",TenQuocGia="Armenia", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Azerbaijan",TenQuocGia="Azerbaijan", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="India",TenQuocGia="Ấn độ", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bahamas",TenQuocGia="Bahamas", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bahrain",TenQuocGia="Bahrain", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bangladesh",TenQuocGia="Bangladesh", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Barbados",TenQuocGia="Barbados", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Belarus",TenQuocGia="Belarus", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Belgium",TenQuocGia="Bỉ", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Belize",TenQuocGia="Belize", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Benin",TenQuocGia="Bénin", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bhutan",TenQuocGia="Bhutan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bolivia",TenQuocGia="Bolivia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="BosniaAndHerzegovina",TenQuocGia="Bosna và Hercegovina", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Botswana",TenQuocGia="Botswana", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Brasil",TenQuocGia="Brasil", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Brunei",TenQuocGia="Brunei", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Bulgaria",TenQuocGia="Bulgaria", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="BurkinaFaso",TenQuocGia="Burkina Faso", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Burundi",TenQuocGia="Burundi", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Cape-Verde",TenQuocGia="Cabo Verde", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Cambodia",TenQuocGia="Campuchia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Cameroon",TenQuocGia="Cameroon", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Canada",TenQuocGia="Canada", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Central-African-Republic",TenQuocGia="Cộng hòa Trung Phi", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Tchad",TenQuocGia="Tchad", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Chile",TenQuocGia="Chile", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="China",TenQuocGia="Trung Quốc", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Colombia",TenQuocGia="Colombia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Comoros",TenQuocGia="Comoros", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Congo-Kinshasa",TenQuocGia="CHDC Congo", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Congo-Brazzaville",TenQuocGia="CH Congo", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="CostaRica",TenQuocGia="Costa Rica", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Croatia",TenQuocGia="Croatia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Cuba",TenQuocGia="Cuba", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Cyprus",TenQuocGia="Síp", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Czech-Republic",TenQuocGia="Séc", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Denmark",TenQuocGia="Đan Mạch", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Djibouti",TenQuocGia="Djibouti", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Dominica",TenQuocGia="Dominica", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Dominican-Republic",TenQuocGia="Cộng hòa Dominica", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Ecuador",TenQuocGia="Ecuador", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="El-Salvador",TenQuocGia="El Salvador", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Eritrea",TenQuocGia="Eritrea", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Estonia",TenQuocGia="Estonia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Poland",TenQuocGia="Ba Lan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Portugal",TenQuocGia="Bồ Đào Nha", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Brazil",TenQuocGia="Brazil", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="CapeVerde",TenQuocGia="Cape Verde", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Timor-Leste",TenQuocGia="Đông Timor", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Germany",TenQuocGia="Đức", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Ethiopia",TenQuocGia="Ethiopia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Georgia",TenQuocGia="Gruzia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Guatemala",TenQuocGia="Guatemala", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="The-Netherlands",TenQuocGia="Hà Lan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Korea",TenQuocGia="Hàn Quốc", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="US",TenQuocGia="Mỹ", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Hungary",TenQuocGia="Hungary", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Indonesia",TenQuocGia="Indonesia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Laos",TenQuocGia="Lào", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Malaysia",TenQuocGia="Malaysia", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Mexico",TenQuocGia="Mexico", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Myanmar",TenQuocGia="Myanmar", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="New-Zealand",TenQuocGia="New Zealand", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Russia",TenQuocGia="Nga", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Japan",TenQuocGia="Nhật Bản", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="France",TenQuocGia="Pháp", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Finland",TenQuocGia="Phần Lan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Philippines",TenQuocGia="Philippines", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Singapore",TenQuocGia="Singapore", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Spain",TenQuocGia="Tây Ban Nha", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Thailand",TenQuocGia="Thái Lan", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Switzerland",TenQuocGia="Thụy Sĩ", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="North-Korea",TenQuocGia="Triều Tiên", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Australia",TenQuocGia="Úc", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
            //new DM_QuocGia{ID=Guid.NewGuid() , MaQuocGia="Italy",TenQuocGia="Ý", GhiChu="", NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}

        };
        #endregion

        public static List<DM_QuocGia> GetQuocGiasKT()
        {
            List<Model.DM_QuocGia> lstQGias = new List<Model.DM_QuocGia>();

            Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
            var lstQGias_MD = dbMacDinh.DM_QuocGia;

            if (lstQGias_MD != null && lstQGias_MD.ToList().Count > 0)
            {
                foreach (Model_banhang24vn.DM_QuocGia objItem_Mau in lstQGias_MD)
                {
                    Model.DM_QuocGia objNew = new DM_QuocGia();
                    objNew.GhiChu = objItem_Mau.GhiChu;
                    objNew.ID = objItem_Mau.ID;
                    objNew.MaQuocGia = objItem_Mau.MaQuocGia;
                    objNew.NgayTao = DateTime.Now;
                    objNew.NguoiTao = "ssoftvn";
                    objNew.TenQuocGia = objItem_Mau.TenQuocGia;

                    lstQGias.Add(objNew);
                }
            }
            return lstQGias;
        }
    }
}
