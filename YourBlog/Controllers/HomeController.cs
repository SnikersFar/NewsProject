using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YourBlog.EfStuff.Repositories;
using YourBlog.Models;

namespace YourBlog.Controllers
{
    public class HomeController : Controller
    {
        ArticleRepository _articleRepository;
        IMapper _mapper;
        public HomeController(ArticleRepository articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        public IActionResult Index(int perPage = 13, int page = 1)
        {
            var CountOfArticles = _articleRepository.Count();
            var articles = _mapper.Map <List<ArticleViewModel>>(_articleRepository.GetForPagination(perPage, page));
            var countOfPages = Math.Ceiling((CountOfArticles * 1.0) / (perPage * 1.0));

            var DataView = new DataArticlesViewModel()
            {
                Articles = articles,
                MyPage = page,
                PerPage = perPage,
                CountPages = Convert.ToInt32(countOfPages),

            };
            return View(DataView);

        }

        public IActionResult InfoArticle(long IdArticle)
        {
            var myArticle = _articleRepository.Get(IdArticle);
            if (myArticle == null)
            {
                return RedirectToAction("Index");
            }
            return View(_mapper.Map<ArticleViewModel>(myArticle));
        }
    }
}
