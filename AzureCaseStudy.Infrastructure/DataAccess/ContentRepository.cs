using AzureCaseStudy.Infrastructure.Model;
using System;
using System.Collections.Generic;

namespace AzureCaseStudy.Infrastructure.DataAccess
{
    public class ContentRepository : IContentRepository
    {
        public IEnumerable<T> GetSitecoreChildren<T>(string parentGuid) where T : class, ISitecoreEntity
        {
            throw new NotImplementedException();
        }

        public T GetSitecoreContentItem<T>(string contentItemGuid) where T : class, ISitecoreEntity
        {
            throw new NotImplementedException();
        }
    }
}
