using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.Domain
{
    public interface IArticlesRepository
    {
        Task<IEnumerable<Article>> GetArticles();
        Task<IEnumerable<Article>> GetArticlesByName(string articleName);
        Task<Article> GetArticleById(int id);
        Task CreateNewArticle(Article article);
        Task UpdateArticleName(int articleId, string newName);
        Task UpdateArticlePrice(int articleId, decimal newPrice);

    }
}