using System.Collections.Generic;
using YourBlog.EfStuff.DbModel;
using YourBlog.EfStuff.Repositories;

namespace YourBlog.Services
{
    public class ArticleService
    {

        private ArticleRepository _articleRepository;

        public ArticleService(ArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }


    }
}
