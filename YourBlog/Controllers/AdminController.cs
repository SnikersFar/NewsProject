using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YourBlog.EfStuff.DbModel;
using YourBlog.EfStuff.Repositories;
using YourBlog.Models;
using YourBlog.Services;

namespace YourBlog.Controllers
{
    public class AdminController : Controller
    {
        private UserRepository _userRepository;
        private UserService _userService;
        private ArticleRepository _articleRepository;
        private IMapper _mapper;

        public AdminController(UserRepository userRepository, UserService userService, IMapper mapper, ArticleRepository articleRepository)
        {
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
            _articleRepository = articleRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel() { ResultLogin = true });
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = _userRepository.GetByNameAndPassword(login, password);
            if (user == null)
            {
                return View(new LoginViewModel() { ResultLogin = true });
            }

            var claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim("Name", user.Name));
            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Startup.AuthCoockieName));

            var claimsIdentity = new ClaimsIdentity(claims, Startup.AuthCoockieName);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Profile", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> Register(string login, string password)
        {
            var userS = _userRepository.GetByNameAndPassword(login, password);
            if (userS != null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var user = new User() { Name = login, Password = password, IsActive = true };
            _userRepository.Save(user);
            var claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim("Name", user.Name));
            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Startup.AuthCoockieName));

            var claimsIdentity = new ClaimsIdentity(claims, Startup.AuthCoockieName);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Profile", "Admin");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var ViewArticles = _mapper.Map<List<ArticleViewModel>>(_userService.GetCurrentUser().Articles.Where(a => a.IsActive).ToList());
            return View(ViewArticles);

        }
        public async Task<IActionResult> Logout(LoginViewModel viewModel)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangeArticle(long idArticle = 0)
        {
            var ChangeArticle = new ChangeArticleViewModel()
            {
                Article = new ArticleViewModel(),
            };
            if (idArticle == 0)
            {
                return View(ChangeArticle);
            }
            var MeUser = _userService.GetCurrentUser();
            var MyArticle = MeUser.Articles.SingleOrDefault(a => a.Id == idArticle && a.IsActive == true);
            if (MyArticle != null)
            {
                ChangeArticle.Article = _mapper.Map<ArticleViewModel>(MyArticle);
                return View(ChangeArticle);
            }
            else
            {
                return RedirectToAction("Profile", "Admin");
            }

        }
        [Authorize]
        [HttpPost]
        public IActionResult ChangeArticle(ArticleViewModel article)
        {
            if (!ModelState.IsValid)
            {
                var ChangeArticle = new ChangeArticleViewModel()
                {
                    Article = article,
                };
                return View(ChangeArticle);
            }
            var MeUser = _userService.GetCurrentUser();
            if (article.CreatorId == 0 || _articleRepository.Get(article.Id).Creator.Id == MeUser.Id)
            {
                var MyArticle = _mapper.Map<Article>(article);
                MyArticle.Creator = MeUser;
                MyArticle.IsActive = true;
                if(article.CreatorId <= 0)
                {
                    MyArticle.CreatedDate = DateTime.Today;
                }
                _articleRepository.Save(MyArticle);
            }

            return RedirectToAction("Profile", "Admin");
        }

        public IActionResult DeleteArticle(long articleId)
        {
            var MeUser = _userService.GetCurrentUser();
            if (MeUser.Articles.Any(a => a.Id == articleId))
            {
                _articleRepository.Remove(articleId);
            }
            return RedirectToAction("Profile", "Admin");
        }

    }

}
