using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SIAProjeto.Filters;
using SIAProjeto.Models;

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
            Utilizador utilizadorAutenticado = db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(System.Web.HttpContext.Current.Session["idUtilizadorAutenticado"]));

            ViewBag.nomeUtilizadorAutenticado = utilizadorAutenticado.nome;

            //Retorna para a view apenas os testes realizados pelo utilizador autenticado
            return View(db.Testes.Where(t => t.idUtilizador == utilizadorAutenticado.idUtilizador));
        }

        public ActionResult Edit()
        {
            return View(db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(System.Web.HttpContext.Current.Session["idUtilizadorAutenticado"])));
        }

        [HttpPost]
        public ActionResult Edit(FormCollection dadosNovos)
        {
            Utilizador utilizadorAutenticado = db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(System.Web.HttpContext.Current.Session["idUtilizadorAutenticado"]));

            if (string.IsNullOrEmpty(dadosNovos["nome"]) == false)
            {
                utilizadorAutenticado.nome = dadosNovos["nome"];
            }

            if (string.IsNullOrEmpty(dadosNovos["password"]) == false)
            {
                if (dadosNovos["password"].Length < 8)
                {
                    ModelState.AddModelError("password", "A palavra-passe introduzida tem que possuir mais de 8 caracteres!");
                }
                else
                {
                    utilizadorAutenticado.password = dadosNovos["password"];
                }
            }

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CreateTeste()
        {
            ViewBag.selectListTecnica = new SelectList(db.Tecnicas.Select(t => t.nome).ToList());

            return View();
        }

        [HttpPost]
        public ActionResult CreateTeste(FormCollection dadosTeste)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosTeste["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem de introduzir o nome para o teste!");
            }

            if (string.IsNullOrEmpty(dadosTeste["idTecnica"]) == true)
            {
                
                ModelState.AddModelError("idTecnica", "Tem de seleccionar uma técnica para o teste!");
            }

            //Se os dados introduzidos estiverem válidos, cria e insere um novo teste na base de dados
            if (ModelState.IsValid == true)
            {
                Teste newTeste = new Teste();

                newTeste.nome = dadosTeste["nome"];
                newTeste.idTecnica = db.Tecnicas.Single(t => t.nome == dadosTeste["idTecnica"]).idTecnica;

                newTeste.idUtilizador = Convert.ToInt32(Session["idUtilizadorAutenticado"]);
                newTeste.dataRealizacao = DateTime.Now;               

                db.Testes.InsertOnSubmit(newTeste);

                db.SubmitChanges();

                return RedirectToAction("PerformTeste", new { id = newTeste.idTeste });
            }
            else
            {
                return View();
            }
        }

        public ActionResult PerformTeste(int id)
        {
            return View();
        }

        public ActionResult Logout()
        {
            //Limpa primeiro todos os dados guardados na sessão do browser
            //Depois atualiza o estado de autenticação do utilizador autenticado na base de dados
            //Por fim, após feito o logout, redirecciona o utilizador para a página de autenticação
            Session.Abandon();

            //Aqui podemos aceder diretamente ao utilizador autenticado sem problemas
            //Isto porque o nosso LoginAcionFilter já verifica se algum ID se encontra presente na sessão do browser, e se esse ID é válido
            db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(System.Web.HttpContext.Current.Session["idUtilizadorAutenticado"])).estadoAutenticacao = false;

            db.SubmitChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}