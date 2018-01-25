using CsvHelper.Configuration;

namespace Sitecore.RedirectionManager.RedirectionObject
{
    public class RedirectionUrlMapObject : ClassMap<RedirectUrlObject>
    {
        public RedirectionUrlMapObject()
        {
            Map(x => x.AcientUrl).Name("Ancient URL");
            Map(x => x.NewUrl).Name("New URL");
        }
    }
}
