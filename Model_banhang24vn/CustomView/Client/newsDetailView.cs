using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView.Client
{
    public class newsDetailView
    {
        public News_Articles NewsModel { get; set; }

        public News_Categories CategoriesModel { get; set; }
        public string CreateByUser { get; set; }
        public List<News_Articles> ListRlatedArticles { get; set; }
    }
}
