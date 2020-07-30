using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    public class AppSettingsService : IAppSettingsService
    {
        public string GoogleMapsApiKey { get; set; }
    }

    public interface IAppSettingsService
    {
        string GoogleMapsApiKey { get; }
    }
}
