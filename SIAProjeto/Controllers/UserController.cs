using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SIAProjeto.Filters;
using SIAProjeto.Models;

namespace SIAProjeto.Controllers
{
    [LoginActionFilter]
    public class UserController : Controller
    {
        DataClassesDBMainDataContext db;

        public UserController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            //Retorna para a view apenas os testes realizados pelo utilizador autenticado
            return View(db.Testes.Where(t => t.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
        }

        public ActionResult Logout()
        {
            //Limpa primeiro todos os dados guardados na sessão do browser
            //Depois atualiza o estado de autenticação do utilizador autenticado na base de dados
            //Por fim, após feito o logout, redirecciona o utilizador para a página de autenticação
            Session.Abandon();

            //Aqui podemos aceder diretamente ao utilizador autenticado sem problemas
            //Isto porque o nosso LoginAcionFilter já verifica se algum ID se encontra presente na sessão do browser, e se esse ID é válido
            db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])).estadoAutenticacao = false;

            db.SubmitChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}