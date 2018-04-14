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

        public ActionResult Create()
        {
            ViewBag.selectListTecnica = new SelectList(db.Tecnicas.Select(t => t.nome).ToList());

            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection dadosTeste)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosTeste["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem de introduzir o nome para o teste!");
            }

            if(string.IsNullOrEmpty(dadosTeste["idTecnica"]) == true)
            {
                ModelState.AddModelError("idTecnica", "Tem de seleccionar uma técnica para o teste!");
            }

            //Se os dados introduzidos estiverem válidos, cria e insere um novo teste na base de dados
            if (ModelState.IsValid == true)
            {
                Teste newTeste = new Teste();

                newTeste.nome = dadosTeste["nome"];
                newTeste.idTecnica = Convert.ToInt32(dadosTeste["idTecnica"]);

                newTeste.idUtilizador = Convert.ToInt32(Session["idUtilizadorAutenticado"]);
                newTeste.dataRealizacao = DateTime.Now;               

                db.Testes.InsertOnSubmit(newTeste);

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditUtilizador()
        {
            return View(db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
        }

        [HttpPost]
        public ActionResult EditUtilizador(FormCollection dadosNovos)
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

            //Se os dados introduzidos estiverem válidos, atualiza o utilizador autenticado com esses mesmos dados
            //Depois submete as alterações na base de dados
            if (ModelState.IsValid == true)
            {
                Utilizador editUtilizador = db.Utilizadors.Single(u => u.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"]));

                editUtilizador.nome = dadosNovos["nome"];
                editUtilizador.password = dadosNovos["password"];

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
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