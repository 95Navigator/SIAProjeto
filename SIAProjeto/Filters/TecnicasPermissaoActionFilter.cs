using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

using SIAProjeto.Models;

namespace SIAProjeto.Filters
{
    public class TecnicasPermissaoActionFilter : ActionFilterAttribute
    {
        DataClassesDBMainDataContext db;

        public TecnicasPermissaoActionFilter()
        {
            db = new DataClassesDBMainDataContext();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Obtém o utilizador autenticado
            Utilizador utilizadorAutenticado = db.Utilizadors.SingleOrDefault(u => u.idUtilizador == Convert.ToInt32(filterContext.HttpContext.Session["idUtilizadorAutenticado"]));

            //Obtém a lista de permissões do tipo do utilizador autenticado
            List<Permissao_TipoUtilizador> permissoesTipoUtilizadorAutenticado = db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == utilizadorAutenticado.idTipoUtilizador).ToList();

            //Se o utilizador tiver concedida a permissão 1 (Gestão de Técnicas), deixa esse mesmo utilizador aceder
            //Se não, bloqueia o acesso a essa parte da plataforma
            if (permissoesTipoUtilizadorAutenticado.SingleOrDefault(pt => pt.idPermissao == 1) != default(Permissao_TipoUtilizador))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/User/Index");
            }
        }
    }
}