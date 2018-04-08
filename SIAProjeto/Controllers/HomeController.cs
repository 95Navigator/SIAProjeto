using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SIAProjeto.Models;

namespace SIAProjeto.Controllers
{
    public class HomeController : Controller
    {
        DataClassesDBMainDataContext db;

        public HomeController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection dadosLogin)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosLogin["email"]) == true)
            {
                ModelState.AddModelError("email", "Introduza o seu e-mail!");
            }

            if (string.IsNullOrEmpty(dadosLogin["password"]) == true)
            {
                ModelState.AddModelError("password", "Introduza a sua palavra-passe!");
            }

            //Se os dados introduzidos estiverem válidos, procura um utilizador que possua o e-mail e a palavra-passe introduzidas
            if (ModelState.IsValid == true)
            {
                Utilizador auxUtilizador = db.Utilizadors.SingleOrDefault(u => u.email == dadosLogin["email"] && u.password == dadosLogin["password"]);

                //Se foi encontrada na base de dados uma e só uma correspondência, guarda o ID desse utilizador encontrado na sessão do browser
                if (auxUtilizador != default(Utilizador))
                {
                    Session["idUtilizadorAutenticado"] = auxUtilizador.idUtilizador;
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Registo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registo(FormCollection dadosRegisto)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if(string.IsNullOrEmpty(dadosRegisto["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome!");
            }

            if(string.IsNullOrEmpty(dadosRegisto["email"]) == true)
            {
                ModelState.AddModelError("email", "Tem que preencher o campo do e-mail!");
            }

            if(string.IsNullOrEmpty(dadosRegisto["password"]) == true)
            {
                ModelState.AddModelError("password", "Tem que preencher o campo da palavra-passe!");
            }

            //Se os dados introduzidos estiverem válidos, cria um novo utilizador com esses mesmos dados
            //Depois submete o novo utilizador na base de dados
            if(ModelState.IsValid == true)
            {
                Utilizador newUtilizador = new Utilizador();

                newUtilizador.nome = dadosRegisto["nome"];
                newUtilizador.email = dadosRegisto["email"];
                newUtilizador.password = dadosRegisto["password"];

                newUtilizador.dataRegisto = DateTime.Now;
                newUtilizador.idTipoUtilizador = 2;

                db.Utilizadors.InsertOnSubmit(newUtilizador);

                db.SubmitChanges();

                return RedirectToAction("Registo");
            }
            else
            {
                return View();
            }
        }
    }
}