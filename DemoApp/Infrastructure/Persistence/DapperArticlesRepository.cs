using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DemoApp.Domain;

namespace DemoApp.Infrastructure.Persistence
{
    public class DapperArticlesRepository : IArticlesRepository
    {
        private readonly IDbConnection _connection;

        public DapperArticlesRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Article>> GetArticles()
        {
            var articles =
                await _connection.QueryAsync<Article>("SELECT * FROM articles");
            return articles;
        }

        public async Task<IEnumerable<Article>> GetArticlesByName(string articleName)
        {
            var articles =
                await _connection.QueryAsync<Article>(
                    "SELECT * FROM articles WHERE name LIKE @articleName"
                    , new {articleName = "%" + articleName + "%"});
            return articles;
        }

        public async Task<Article> GetArticleById(int id)
        {
            var article = await _connection.QuerySingleAsync<Article>(
                "SELECT * FROM articles WHERE articleId = @articleId",
                new {articleId = id});
            return article;
        }

        public async Task CreateNewArticle(Article article)
        {
            await _connection.QueryAsync(
                @"INSERT INTO articles(name,description,price,created) 
                        VALUES(@Name, @Description, @Price, @Created)", article);
        }

        public Task UpdateArticlePrice(int articleId, decimal newPrice)
        {
            throw new NotImplementedException();
        }

        public Task UpdateArticleName(int articleId, string newName)
        {
            throw new NotImplementedException();
        }
    }
}