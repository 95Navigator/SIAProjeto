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
    [TiposUtilizadorPermissaoActionFilter]
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
            //Compõe o ViewModel para a página de detalhes do tipo de utiizador
            TiposUtilizadorViewModel detailsViewModel = new TiposUtilizadorViewModel();

            detailsViewModel.TipoUtilizador = db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id);         

            List<Permissao_TipoUtilizador> permissoesTipoUtilizadorList = db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id).ToList();

            foreach(Permissao_TipoUtilizador pt in permissoesTipoUtilizadorList)
            {
                detailsViewModel.PermissoesList.Add(db.Permissaos.Single(p => p.idPermissao == pt.idPermissao));
            }

            return View(detailsViewModel);
        }

        public ActionResult Create()
        {
            TiposUtilizadorViewModel createViewModel = new TiposUtilizadorViewModel();

            createViewModel.PermissoesList = db.Permissaos.ToList();

            return View(createViewModel);
        }

        [HttpPost]
        public ActionResult Create(FormCollection dadosTipoUtilizador)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosTipoUtilizador["TipoUtilizador.nome"]) == true)
            {
                ModelState.AddModelError("TipoUtilizador.nome", "Tem de introduzir um nome para o novo tipo de utilizador!");
            }
            else
            {
                //Se já existe um tipo de utilizador com o nome introduzido, retorna erro
                if (db.TipoUtilizadors.SingleOrDefault(t => t.nome == dadosTipoUtilizador["nome"]) != default(TipoUtilizador))
                {
                    ModelState.AddModelError("TipoUtilizador.nome", "Um tipo de utilizador com este nome já se encontra criado!");
                }
            }

            if(ModelState.IsValid == true)
            {
                TipoUtilizador newTipoUtilizador = new TipoUtilizador();

                newTipoUtilizador.nome = dadosTipoUtilizador["TipoUtilizador.nome"];

                db.TipoUtilizadors.InsertOnSubmit(newTipoUtilizador);

                db.SubmitChanges();

                foreach(Permissao p in db.Permissaos)
                {
                    //Visto que o ASP.NET MVC faz umas coisas esquisitas com as checkboxes, este workaround certifica que a checkbox de cada permissão é corretamente validada
                    if (dadosTipoUtilizador[string.Concat("permissao", p.idPermissao)].Contains("true") == true)
                    {
                        Permissao_TipoUtilizador newPermissaoTipoUtilizador = new Permissao_TipoUtilizador();

                        newPermissaoTipoUtilizador.idTipoUtilizador = newTipoUtilizador.idTipoUtilizador;
                        newPermissaoTipoUtilizador.idPermissao = p.idPermissao;

                        db.Permissao_TipoUtilizadors.InsertOnSubmit(newPermissaoTipoUtilizador);
                    }
                }

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TiposUtilizadorViewModel createViewModel = new TiposUtilizadorViewModel();

                createViewModel.PermissoesList = db.Permissaos.ToList();

                return View(createViewModel);
            }
        }

        public ActionResult Edit(int id)
        {
            //Compõe o ViewModel para a página de edição do tipo de utiizador
            TiposUtilizadorViewModel editViewModel = new TiposUtilizadorViewModel();

            editViewModel.TipoUtilizador = db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id);
            editViewModel.PermissoesTipoUtilizadorList = db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id).ToList();
            editViewModel.PermissoesList = db.Permissaos.ToList();

            return View(editViewModel);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection dadosTipoUtilizador, int id)
        {
            //Obtém, de acordo com o ID, o tipo de utilizador respetivo para editar
            TipoUtilizador editTipoUtilizador = db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id);

            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosTipoUtilizador["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem de introduzir um nome para o novo tipo de utilizador!");
            }
            else
            {
                //Se o nome do tipo de utilizador introduzido não for igual ao que estava, mas se já existe um tipo de utilizador com o novo nome introduzido, retorna erro
                if (editTipoUtilizador.nome != dadosTipoUtilizador["nome"] && db.TipoUtilizadors.SingleOrDefault(t => t.nome == dadosTipoUtilizador["nome"]) != default(TipoUtilizador))
                {
                    ModelState.AddModelError("nome", "Um tipo de utilizador com este nome já se encontra criado!");
                }
            }

            if (ModelState.IsValid == true)
            {
                editTipoUtilizador.nome = dadosTipoUtilizador["nome"];

                db.Permissao_TipoUtilizadors.DeleteAllOnSubmit(db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id));
                db.SubmitChanges();

                foreach (Permissao p in db.Permissaos)
                {
                    //Visto que o ASP.NET MVC faz umas coisas esquisitas com as checkboxes, este workaround certifica que a checkbox de cada permissão é corretamente validada
                    if (dadosTipoUtilizador[string.Concat("permissao", p.idPermissao)].Contains("true") == true)
                    {
                        Permissao_TipoUtilizador newPermissaoTipoUtilizador = new Permissao_TipoUtilizador();

                        newPermissaoTipoUtilizador.idTipoUtilizador = editTipoUtilizador.idTipoUtilizador;
                        newPermissaoTipoUtilizador.idPermissao = p.idPermissao;

                        db.Permissao_TipoUtilizadors.InsertOnSubmit(newPermissaoTipoUtilizador);
                    }
                }

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                //Compõe o ViewModel para a página de edição do tipo de utiizador
                TiposUtilizadorViewModel editViewModel = new TiposUtilizadorViewModel();

                editViewModel.TipoUtilizador = db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id);
                editViewModel.PermissoesTipoUtilizadorList = db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id).ToList();
                editViewModel.PermissoesList = db.Permissaos.ToList();

                return View(editViewModel);
            }
        }

        public ActionResult Delete(int id)
        {
            //Compõe o ViewModel para a página de eliminação do tipo de utiizador
            TiposUtilizadorViewModel deleteViewModel = new TiposUtilizadorViewModel();

            deleteViewModel.TipoUtilizador = db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id);
            deleteViewModel.PermissoesList = new List<Permissao>();

            List<Permissao_TipoUtilizador> permissoesTipoUtilizadorList = db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id).ToList();

            foreach (Permissao_TipoUtilizador pt in permissoesTipoUtilizadorList)
            {
                deleteViewModel.PermissoesList.Add(db.Permissaos.Single(p => p.idPermissao == pt.idPermissao));
            }

            return View(deleteViewModel);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection dummy, int id)
        {
            //Primeiro verifica se este tipo de utilizador possui permissões associadas a ele
            //Se possuir, apaga essas mesmas relações
            db.Permissao_TipoUtilizadors.DeleteAllOnSubmit(db.Permissao_TipoUtilizadors.Where(pt => pt.idTipoUtilizador == id));

            //Depois de todas as relações apagadas, elimina o próprio tipo de utilizador
            db.TipoUtilizadors.DeleteOnSubmit(db.TipoUtilizadors.Single(t => t.idTipoUtilizador == id));

            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}