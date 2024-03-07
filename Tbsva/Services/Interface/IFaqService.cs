using WebShopping.Helpers;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Services
{
    public interface IFaqService
    {            
        Faq GetFaqData(int id);

        List<Faq> GetFaqSetData();

        Faq InsertFaq(HttpRequest request);

        void UpdateFaq(HttpRequest request, Faq faq);

        void DeleteFaq(Faq faq);
    }
}
