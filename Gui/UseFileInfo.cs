using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerLoganAndHisCowCalledLoui.Gui
{
    [Serializable()]
    public class UseFileInfo
    {
        public string PathFile { get; set; }
        public DateTime TimeOfOpening { get; set; }

        internal static UseFileInfo Create(string pathfile, DateTime timeOfOpening)
        {
            var ret = new UseFileInfo();
            ret.Set(pathfile, timeOfOpening);
            return ret;
        }

        private void Set(string pathfile, DateTime timeOfOpening)
        {
            this.PathFile = pathfile;
            this.TimeOfOpening = timeOfOpening;
        }
    }
}
