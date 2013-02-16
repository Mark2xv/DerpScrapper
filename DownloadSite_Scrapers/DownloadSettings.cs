using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper.DownloadSite_Scrapers
{
    static class DownloadSettings
    {
        public static bool MustAbide = false;
        public static QualityInformation PrefferedDownloadQuality
        {
            get
            {
                return new QualityInformation(QualityInformation.Source.BlueRay, QualityInformation.Resolution.HD1080, QualityInformation.Encoding.TenBit);
            }
        }
    }
}
