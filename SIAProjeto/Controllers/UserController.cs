using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SIAProjeto.Filters;
using SIAProjeto.Models;
using SIAProjeto.ViewModels;

namespace SIAProjeto.Controllers
{
    [RequireHttps]
    [LoginActionFilter]
    public class UserController : Controller
    {
        private DataClassesDBMainDataContext db;

        public UserController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            UserViewModel indexViewModel = new UserViewModel();

            indexViewModel.Utilizador = db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"]));
            indexViewModel.TestesList = db.Testes.Where(t => t.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])).ToList();

            return View(indexViewModel);
        }

        public ActionResult Edit()
        {
            return View(db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
        }

        [HttpPost]
        public ActionResult Edit(FormCollection dadosNovos)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome!");
            }

            if (string.IsNullOrEmpty(dadosNovos["password"]) == true)
            {
                ModelState.AddModelError("password", "Tem que preencher o campo da palavra-passe!");
            }
            else
            {
                if(dadosNovos["password"].Length < 8)
                {
                    ModelState.AddModelError("password", "A palavra-passe introduzida tem que possuir mais de 8 caracteres!");
                }
            }

            //Se os dados introduzidos estiverem válidos, obtém o utilizador autenticado a partir do seu respetivo ID guardado na sessão
            //Depois de associar os novos dados introduzidos a esse utiizador, submete as mudanças na base de dados
            if (ModelState.IsValid == true)
            {
                Utilizador utilizadorAutenticado = db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"]));

                utilizadorAutenticado.nome = dadosNovos["nome"];
                utilizadorAutenticado.password = dadosNovos["password"];

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
            }
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