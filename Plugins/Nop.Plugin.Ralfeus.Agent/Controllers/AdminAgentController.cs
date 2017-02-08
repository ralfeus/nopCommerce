using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Ralfeus.Agent.Controllers
{
    public class AdminAgentController : BasePluginController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult AgentList()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult UpdateAgent()
        {
            throw new System.NotImplementedException();
        }
    }
}