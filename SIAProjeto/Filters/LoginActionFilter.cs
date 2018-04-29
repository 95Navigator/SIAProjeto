using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

using SIAProjeto.Models;

namespace SIAProjeto.Filters
{
    public class LoginActionFilter : ActionFilterAttribute
    {
        DataClassesDBMainDataContext db;

        public LoginActionFilter()
        {
            db = new DataClassesDBMainDataContext();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Verifica se existe um ID qualquer presente na sessão do browser
            //Depois também verifica se esse ID corresponde a um utilizador presente na base de dados
            //Se ambas as condições forem cumpridas, a página pode ser apresentada; caso contrário o utilizador é redireccionado para a página de autenticação
            if (filterContext.HttpContext.Session["idUtilizadorAutenticado"] != null && db.Utilizadors.SingleOrDefault(u => u.idUtilizador == Convert.ToInt32(filterContext.HttpContext.Session["idUtilizadorAutenticado"])) != default(Utilizador))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/Home/Index");
            }
        }
    }
}