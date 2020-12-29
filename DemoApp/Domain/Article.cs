using System;
using System.Numerics;

namespace DemoApp.Domain
{
    public class Article
    {
        //TODO Make this a self-generated ID. Remove dependency to DB-ID to have a real domain object.
        //but well. works for v0.1 :)
        public BigInteger ArticleId { get; set; }
        
        //TODO Value Objects (records?)
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}