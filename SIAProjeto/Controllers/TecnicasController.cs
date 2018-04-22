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



            //Falta: seleccioncar um quadrante  já feito com perguntas 
            //id selecionado no fropdown já aqui vem; 
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Deve Introduzir um nome válido para a técnica");
            }
            
            if (ModelState.IsValid == true)
            {
                Tecnica newTecnica = new Tecnica();

                newTecnica.nome = dadosNovos["nome"];

                //newTecnica.FlsComplete = dadosNovos["FlsComplete"]; 
                //inser a "newTecnica" no conjunto de Dados na base de dados; 
                db.Tecnicas.InsertOnSubmit(newTecnica);
                db.SubmitChanges();

                Quadrante newQuadrante = new Quadrante();

                //db.Quadrantes.Select(Func<Quadrante, idquadrante>);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditarTecnica(int id, FormCollection dadosNovos)
        {
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome!");
            }

            if (string.IsNullOrEmpty(dadosNovos["FlsComplete"]) == false)
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
            //apresenta o ecra dos quadrantes com um botao para listar as perguntas 
            //retornar para a view a tecnica, cujo o id é "id"; 
            return View(db.Tecnicas.Single(t=>t.idTecnica == id));
        }


        // qual diferença entre dois ??? 
        public ActionResult DeleteTecnicas(int idTecnica)
        {
            // apagar apenas a ténica, não apagar quadrantes nem perguntas associadas
            //não apaga pergunta nem quadrante, apenas a técnica 
            return View(db.Tecnicas.Single(t=>t.idTecnica==idTecnica));
        }

        [HttpPost]
        public ActionResult DeleteTecnica(FormCollection fake, int idTecnica)
        {
            db.Tecnicas.DeleteOnSubmit(db.Tecnicas.Single(t => t.idTecnica == idTecnica));
            db.SubmitChanges(); //função que carrega "Ok" em todas as mudanças que queremos realizar no repositorio de dados; 

            return RedirectToAction("Index"); 
        }
        #endregion
        
        /// <summary>
        /// É criada uma lista de "Quadrante_Texnicas", a qual vai ser tornar numa coleção desse objeto da base de dados, 
        /// Entretanto a lista de "Quadrante", repreenta uma linha da coleção aux
        /// Ciclo foreach pega e para cada linha da coleção "aux", onde o id de "aux" é igual ao id de "qt" (linha da coleção), é acrescentado esse mesmo elemento/linha
        /// por ultimoé retornada a Lista desses esmos quadrantes. 
        /// </summary>
        /// <param name="idTecnica"></param>
        /// <returns></returns>

        public ActionResult ListQuadrantes(int idTecnica)
        {
            List<Quadrante_Tecnica> aux = db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == idTecnica).ToList();

            List<Quadrante> aux2 = new List<Quadrante>();

            foreach(Quadrante_Tecnica qt in aux)
            {
                aux2.Add(db.Quadrantes.Single(q => q.idQuadrante == qt.idQuadrante));
            }

            return View(aux2);
        }

        public ActionResult ListPerguntas(int idQuadrante)
        {
            List<Pergunta_Quadrante> aux = db.Pergunta_Quadrantes.Where(pq => pq.idPergunta_Quadrante == idQuadrante).ToList();

            List<Pergunta> aux2 = new List<Pergunta>();

            foreach (Pergunta_Quadrante pq in aux)
            {
                aux2.Add(db.Perguntas.Single(p=>pq.idPergunta == pq.idPergunta));
            }

            return View(aux2);
        }

        #region Pergunta 
        public ActionResult CriarPergunra(FormCollection dadosNovos)
        {
            //criação de nova pergunta; 
            //fica criada uma pullde perguntas e pull de quadrantes 
            return null; 
        }
        public ActionResult EditarPergunta(FormCollection dadosNovos, int id)
        {
            return View(db.Perguntas.Single(p => p.idPergunta == id));

        }
        public ActionResult DetalhesPergunta(FormCollection dadosNovos)
        {
            //detalhes de nova pergunta; 
            return null; 
        }

        //public ActionResult DeletePergunta(int idPergunta)
        //{
        //    //return View(db.Perguntas.Single(p=>p.));
        //}

        [HttpPost]
        public ActionResult DeletePergunta(FormCollection fake, int idPergunta)
        {
            db.Perguntas.DeleteOnSubmit(db.Perguntas.Single(p => p.idPergunta == idPergunta));
            db.SubmitChanges(); //função que carrega "Ok" em todas as mudanças que queremos realizar no repositorio de dados; 

            return RedirectToAction("Index");
        }
        #endregion

        #region Quadrante
        public ActionResult CriarQuadrante (FormCollection dadosNovos)
        {
            //criação de nova pergunta; 
            return null;
        }
        public ActionResult EditarQuadrante(FormCollection dadosNovos, int id)
        {
            return View(db.Quadrantes.Single(q=>q.idQuadrante==id));
        }
        public ActionResult DetalhesQuadrante(int idQuadrante)
        {
            //detalhes de nova pergunta; 
            return View(db.Quadrantes.Single(q => q.idQuadrante == idQuadrante));
        }

        [HttpPost]
        public ActionResult DeleteQuadrante(FormCollection fake, int idQuadrante)
        {
            db.Quadrantes.DeleteOnSubmit(db.Quadrantes.Single(q => q.idQuadrante == idQuadrante));
            db.SubmitChanges(); //função que carrega "Ok" em todas as mudanças que queremos realizar no repositorio de dados; 

            return RedirectToAction("Index");

        }

    
        #endregion


    }
}