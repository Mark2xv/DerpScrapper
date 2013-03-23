using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DerpScrapper.DBO;

namespace DerpScrapper.ResourceSite_Scrapers
{
    class TVRage : IScraper
    {
        string baseUrl = "http://services.tvrage.com/feeds/search.php?show={0}";


        public SerieInfo FindAllInformationForSerie(string serieName)
        {
            throw new NotImplementedException();
        }

        public DBO.ResourceSite GetResourceSite()
        {
            ResourceSite site = new ResourceSite(2);

            return site;
        }
    }
}
