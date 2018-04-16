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
    public class TiposUtilizadorController : Controller
    {
        private DataClassesDBMainDataContext db;

        public TiposUtilizadorController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            return View(db.TipoUtilizadors);
        }

        public ActionResult Details(int id)
        {
            return View(db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection dadosTipoUtilizador)
        {
            if(string.IsNullOrEmpty(dadosTipoUtilizador["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem de introduzir um nome para o novo tipo de utilizador!");
            }
            else
            {
                if(db.TipoUtilizadors.SingleOrDefault(t => t.nome == dadosTipoUtilizador["nome"]) != default(TipoUtilizador))
                {
                    ModelState.AddModelError("nome", "Um tipo de utilizador com este nome já se encontra criado!");
                }
            }

            if(ModelState.IsValid == true)
            {
                TipoUtilizador newTipoUtilizador = new TipoUtilizador();

                newTipoUtilizador.nome = dadosTipoUtilizador["nome"];

                db.TipoUtilizadors.InsertOnSubmit(newTipoUtilizador);

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View(db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection dummy, int id)
        {
            db.TipoUtilizadors.DeleteOnSubmit(db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id));

            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}