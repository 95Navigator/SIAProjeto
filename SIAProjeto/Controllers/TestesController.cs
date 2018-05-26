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
    public class TestesController : Controller
    {
        private DataClassesDBMainDataContext db;  

        public TestesController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            return View(db.Testes.Where(t => t.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])));
        }

        public ActionResult Create()
        {
            ViewBag.selectListTecnicas = new SelectList(db.Tecnicas.Select(t => t.nome).ToList());

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
                newTeste.idTecnica = 1;

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

        public ActionResult Delete(int id)
        {
            return View(db.Testes.Single(t => t.idTeste == id));
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection dummy)
        {
            db.Testes.DeleteOnSubmit(db.Testes.Single(t => t.idTeste == id));

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult PerformTeste(int id)
        {
            TestesViewModel performTesteViewModel = new TestesViewModel();

            performTesteViewModel.Teste = db.Testes.Single(t => t.idTeste == id);
            performTesteViewModel.Tecnica = db.Tecnicas.Single(t => t.idTecnica == performTesteViewModel.Teste.idTecnica);

            Quadrante_Tecnica currentQuadranteTecnica = db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == performTesteViewModel.Tecnica.idTecnica).First();
            Quadrante currentQuadrante = db.Quadrantes.Single(q => q.idQuadrante == currentQuadranteTecnica.idQuadrante);

            performTesteViewModel.Quadrante = currentQuadrante;

            List<Pergunta_Quadrante> currentPerguntasQuadrante = db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == currentQuadrante.idQuadrante).ToList();

            foreach (Pergunta_Quadrante pq in currentPerguntasQuadrante)
            {
                performTesteViewModel.PerguntasList.Add(db.Perguntas.Single(p => p.idPergunta == pq.idPergunta));
            }

            return View(performTesteViewModel);
        }

        [HttpPost]
        public ActionResult PerformTeste(int id, FormCollection dadosPontuacoes)
        {
            foreach(Pergunta_Quadrante pq in db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id))
            {
                if(string.IsNullOrEmpty(dadosPontuacoes["pontuacao"] + pq.idPergunta) == true)
                {
                    ModelState.AddModelError("pontuacao" + pq.idPergunta, "Tem que introduzir uma pontuação para esta pergunta!");
                }
                else
                {
                    if(Convert.ToInt32(dadosPontuacoes["pontuacao"] + pq.idPergunta) > 10 || Convert.ToInt32(dadosPontuacoes["pontuacao"] + pq.idPergunta) < 0)
                    {
                        ModelState.AddModelError("pontuacao" + pq.idPergunta, "A pontuação tem que ser um valor inteiro de 0 a 10!");
                    }
                }
            }

            if(ModelState.IsValid == true)
            {
                return RedirectToAction("PerformTeste", new { id = id });
            }
            else
            {
                return View();
            }
        }

        public ActionResult SaveTeste()
        {
            return View();
        }
    }
}