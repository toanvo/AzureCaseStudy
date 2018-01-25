using System;
using Sitecore.Data;

namespace AzureCaseStudy.Infrastructure.Model
{
    public class SitecoreEntity : ISitecoreEntity
    {   
        public ID Id { get; set; }

        public SitecoreEntity() { }

        public SitecoreEntity(Guid id)
        {
            this.Id = new ID(id);
        }
    }
}
