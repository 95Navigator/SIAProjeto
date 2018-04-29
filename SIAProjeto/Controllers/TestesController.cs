using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SIAProjeto.Models;
using SIAProjeto.ViewModels;

namespace SIAProjeto.Controllers
{
    public class TestesController : Controller
    {
        DataClassesDBMainDataContext db;

        public TestesController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
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

            //Se os dados introduzidos estiverem válidos, cria e insere um novo teste na base de dados
            if (ModelState.IsValid == true)
            {
                Teste newTeste = new Teste();

                newTeste.nome = dadosTeste["nome"];

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
    }
}