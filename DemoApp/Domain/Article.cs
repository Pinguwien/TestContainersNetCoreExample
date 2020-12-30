using System;
using System.Numerics;

namespace DemoApp.Domain
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}