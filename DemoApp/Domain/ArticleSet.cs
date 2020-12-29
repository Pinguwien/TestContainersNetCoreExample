using System.Collections.Generic;

namespace DemoApp.Domain
{
    //TODO
    public class ArticleSet
    {
        public string Name { get; set; }
        public List<Article> Articles { get; set; }
        public decimal Price { get; set; }
    }
}