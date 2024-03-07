using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Helpers
{
    public interface IImageFormatHelper
    {
        bool CheckImageMIME(HttpFileCollection fileCollection);

        bool CheckImageMIME2(HttpFileCollection fileCollection);
    }
}
