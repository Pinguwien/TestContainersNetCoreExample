using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoApp.Api
{
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

        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            Logger.LogDebug("Received a GET request for All Articles");

            var result = await _articlesRepository.GetArticles();

            return Ok(result);
        }

        [HttpGet("Protected")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Article>>> GetProtectedArticles()
        {
            Logger.LogDebug("Received a GET request for Protected Articles");

            var result = await _articlesRepository.GetArticles();

            return Ok(result);
        }
    }
}