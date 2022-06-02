namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20211005_4 : DbMigration
    {
        public override void Up()
        {
            Sql(@"update hh set LoaiHangHoa = iif (hh.LaHangHoa = 1, 1, 2)
            from DM_HangHoa hh
            where hh.LoaiHangHoa is null or hh.LoaiHangHoa  = 0");

        }
        
        public override void Down()
        {
        }
    }
}
