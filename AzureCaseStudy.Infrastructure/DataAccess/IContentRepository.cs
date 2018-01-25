using AzureCaseStudy.Infrastructure.Model;
using System.Collections.Generic;

namespace AzureCaseStudy.Infrastructure.DataAccess
{
    public interface IContentRepository
    {
        T GetSitecoreContentItem<T>(string contentItemGuid) where T : class, ISitecoreEntity;
        IEnumerable<T> GetSitecoreChildren<T>(string parentGuid) where T : class, ISitecoreEntity;
    }
}
