using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    internal class ApplicationSetting
    {
        public static ApplicationSetting Instance => _instance.Value;
        private static Lazy<ApplicationSetting> _instance => new Lazy<ApplicationSetting>(() => new ApplicationSetting());
        private ApplicationSetting() { }


        public void SaveWindowSize(int w, int h)
        {

        }

        public void SaveHome(string url)
        {

        }

        public void SaveSearchHistory(string url)
        {

        }
    }
}
