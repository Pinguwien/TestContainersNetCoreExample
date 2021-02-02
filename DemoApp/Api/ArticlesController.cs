using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoApp.Api
{
    //TODO    [Route("api/[controller]")]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private readonly ILogger Logger;

        //Move to Article-class(?)
        private readonly IArticlesRepository _articlesRepository;

        public ArticlesController(IArticlesRepository articlesRepository, ILogger<ArticlesController> logger)
        {
            _articlesRepository = articlesRepository;
            this.Logger = logger;
        }

        //getArticles
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            Logger.LogDebug("Received a GET request for All Articles");

            var result = await _articlesRepository.GetArticles();

            return Ok(result);
        }

        //getArticle ?name= ?price= ...
    }
}