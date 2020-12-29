using DemoApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Api
{
    //TODO
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesRepository _articlesRepository;

        public ArticlesController(IArticlesRepository articlesRepository)
        {
            _articlesRepository = articlesRepository;
        }

        //getArticles
        //getArticle ?name= ?price= ...
    }
}