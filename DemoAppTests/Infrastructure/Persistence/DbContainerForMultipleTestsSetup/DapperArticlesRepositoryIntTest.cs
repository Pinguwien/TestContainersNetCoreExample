using System;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.Infrastructure.Persistence;
using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence.DbContainerForMultipleTestsSetup
{
    public class DapperArticlesRepositoryIntTest
    {
        [Test]
        public async Task SucceedsWhenGetArticlesReturnsTestArticle()
        {
            const int expectedCount = 5;
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString));

            var result = (await cut.GetArticles()).ToList();
            Assert.That(result, Has.Count.EqualTo(expectedCount));
            Assert.That(result.First().Name, Is.EqualTo("article1"));
        }

        [Test]
        public async Task SucceedsWhenSearchForPartOfNameOfAllArticlesReturnsFiveArticles()
        {
            const int expectedCount = 5;
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString));

            var result = (await cut.GetArticlesByName("artic")).ToList();
            Assert.That(result, Has.Count.EqualTo(expectedCount));
        }

        [Test]
        public async Task SucceedsWhenSearchForArticleFiveOnlyReturnsArticleFive()
        {
            const int expectedCount = 1;
            const string nameParam = "article5";
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString));

            var articles = (await cut.GetArticlesByName(nameParam)).ToList();
            Assert.That(articles, Has.Count.EqualTo(expectedCount));
            Assert.That(articles.First().Name, Is.EqualTo(nameParam));
        }

        [Test]
        public async Task SucceedsWhenArticleByIdReturnsRightArticle()
        {
            const int idParam = 3;
            const string expectedName = "article3";
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString));

            var article = await cut.GetArticleById(idParam);
            Assert.That(article.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void SucceedsWhenInvalidOperationExceptionIsThrownWhenGetArticleByIdIsCalledWithNonExistentId()
        {
            const int idParam = 999;
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString));

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                cut.GetArticleById(idParam));
        }
    }
}