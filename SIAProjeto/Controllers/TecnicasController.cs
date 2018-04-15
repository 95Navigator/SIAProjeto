using SIAProjeto.Filters;
using SIAProjeto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAProjeto.Controllers
{
    /// <summary>
    /// Controlador responsável por ténicas, perguntas e quadrantes dado que tudo se engloba dentro de uma Ténica. 
    /// 
    /// </summary>
    /// <returns></returns>
    [LoginActionFilter] // Não deica que um utilizador não autenticado, aceda a este Controlador; 
    public class TecnicasController : Controller
    {
        DataClassesDBMainDataContext db;

        public TecnicasController() // Construtor 
        {
            //ligação à base de dados 
            db = new DataClassesDBMainDataContext();            
        }

        // GET: Tecnicas
        // o que vai retornar ma View 
        public ActionResult Index()
        {
            ViewBag.TotalTecnicas = db.Tecnicas.Count();
            ViewBag.TotalPerguntas = db.Perguntas.Count(); 
            ViewBag.TotalQuadrantes = db.Quadrantes.Count();
            return View(db.Tecnicas.Where(t => t.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
        }

        [HttpPost]

        #region Tecnica
        public ActionResult CriarTecnica(FormCollection dadosNovos)
        {
            bool aux = false;

            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Deve Introduzir um nome válido para a técnica");
            }

            if (string.IsNullOrEmpty(dadosNovos["idTeste"]) == true)
            {
                ModelState.AddModelError("idTEste", "Deve Introduzir um teste válido para a técnica em uso!!");
            }

            //Pergunta 
            if (string.IsNullOrEmpty(dadosNovos["texto"]) == true)
            {
                ModelState.AddModelError("texto", "Deve Introduzir um teste válido para a técnica em uso!!");
            }

            if (string.IsNullOrEmpty(dadosNovos["importancia"]) == true)
            {
                ModelState.AddModelError("importancia", "Deve Introduzir um teste válido para a técnica em uso!!");
            }

            //Quadrante 
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Deve Introduzir um teste válido para a técnica em uso!!");
            }

            //se campos todos preenchidos o valor da tabela "FlsComplete" é alterao para "1"
            if ((string.IsNullOrEmpty(dadosNovos["idTeste"]) == true) && (string.IsNullOrEmpty(dadosNovos["nome"]) == true))
                aux = true;

            if (ModelState.IsValid == true)
            {
                Tecnica newTecnica = new Tecnica();

                newTecnica.nome = dadosNovos["nome"];
                //newTecnica.FlsComplete = dadosNovos["FlsComplete"]; 
                //inser a "newTecnica" no conjunto de Dados na base de dados; 
                db.Tecnicas.InsertOnSubmit(newTecnica);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        public ActionResult EditarTecnica(int id)
        {

            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome!");
            }

            if (string.IsNullOrEmpty(dadosNovos["FlsComplete"]) == true)
            {
                ModelState.AddModelError("FlsComplete", "Algum campo está em falta!");
            }

            //Se os dados introduzidos estiverem válidos, atualiza o utilizador autenticado com esses mesmos dados
            //Depois submete as alterações na base de dados
            if (ModelState.IsValid == true)
            {
                Tecnica editTecnica = db.Tecnicas.Single(t => t.idTecnica == Convert.ToInt32(Session["idTecnica"]));

                editTecnica.nome = dadosNovos["nome"];
                //editTecnica.FlsComplete = dadosNovos["FlsComplete"];
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult DetalhesTecnicas(int id)
        {

            //retornar para a view a tecnica, cujo o id é "id"; 
            return View();
        }

        public ActionResult DeleteTecnicas(FormCollection dadosNovos)
        {

            return View();
        }
        #endregion

        public ActionResult ShowGrafico(FormCollection collection)
        {
            return null; 
        }

        public ActionResult ListaPerguntas(FormCollection collection)
        {


            return null; 
        }


  #region Pergunta 
        public ActionResult CriarPergunra(FormCollection dadosNovos)
        {
            //criação de nova pergunta; 
            return null; 
        }
        public ActionResult EditarPergunta(FormCollection dadosNovos)
        {

            return null;
        }
        public ActionResult DetalhesPergunta(FormCollection dadosNovos)
        {
            //detalhes de nova pergunta; 
            return null; 
        }


        public ActionResult DeletePergunta(FormCollection dadosNovos)
        {

            return null; 
        }
        #endregion

        #region Quadrante
        public ActionResult CriarQuadrante (FormCollection dadosNovos)
        {
            //criação de nova pergunta; 
            return null;
        }
        public ActionResult EditarQuadrante(FormCollection dadosNovos)
        {

            return null;
        }
        public ActionResult DetalhesQuadrante(FormCollection dadosNovos)
        {
            //detalhes de nova pergunta; 
            return null;
        }


        public ActionResult DeleteQuadrante(FormCollection dadosNovos)
        {

            return null;
        }
        #endregion


    }
}