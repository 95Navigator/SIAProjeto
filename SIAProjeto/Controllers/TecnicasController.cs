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
    [LoginActionFilter] // Não deica que um utilizador não autenticado aceda a este Controlador; 
    [TecnicasPermissaoActionFilter]
    public class TecnicasController : Controller
    {
        private DataClassesDBMainDataContext db;       

        public TecnicasController() // Construtor 
        {
            //ligação à base de dados 
            db = new DataClassesDBMainDataContext();
        }

        // o que vai retornar ma View 
        public ActionResult Index()
        {
            TecnicasViewModel indexViewModel = new TecnicasViewModel();

            indexViewModel.TecnicasList = db.Tecnicas.Where(t => t.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])).ToList();
            indexViewModel.QuadrantesList = db.Quadrantes.Where(q => q.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])).ToList();
            indexViewModel.PerguntasList = db.Perguntas.Where(p => p.idUtilizador == Convert.ToInt32(Session["idUtilizadorAutenticado"])).ToList();

            return View(indexViewModel);
        }

        #region Tecnica
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection dadosNovos)
        {
            //Falta: seleccioncar um quadrante  já feito com perguntas 
            //id selecionado no dropdown já aqui vem; 
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Deve Introduzir um nome válido para a técnica");
            }
            
            if (ModelState.IsValid == true)
            {
                Tecnica newTecnica = new Tecnica();

                newTecnica.nome = dadosNovos["nome"];
                newTecnica.idUtilizador = Convert.ToInt32(Session["idUtilizadorAutenticado"]);

                //newTecnica.FlsComplete = dadosQuadrante["FlsComplete"]; 
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

        public ActionResult Details(int id)
        {
            TecnicasViewModel detailsViewModel = new TecnicasViewModel();

            detailsViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == id));

            return View(detailsViewModel);
        }

        public ActionResult Edit(int id)
        {
            TecnicasViewModel editViewModel = new TecnicasViewModel();

            //A técnica que nós queremos é a técnica correspondente ao ID passado como parâmetro
            editViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == id));
            editViewModel.QuadrantesTecnicaList = db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == id).ToList();
            editViewModel.QuadrantesList = db.Quadrantes.ToList();
            editViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.ToList();
            editViewModel.PerguntasList = db.Perguntas.ToList();   

            return View(editViewModel);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection dadosNovos, int id)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome da técnica!");
            }

            //Se os dados introduzidos estiverem válidos, atualiza a técnica pretendida com esses mesmos dados
            //Depois submete as alterações na base de dados
            if (ModelState.IsValid == true)
            {
                Tecnica editTecnica = db.Tecnicas.Single(t => t.idTecnica == id);         

                editTecnica.nome = dadosNovos["nome"];

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TecnicasViewModel editViewModel = new TecnicasViewModel();

                //A técnica que nós queremos é a técnica correspondente ao ID passado como parâmetro
                editViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == id));
                editViewModel.QuadrantesTecnicaList = db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == id).ToList();
                editViewModel.QuadrantesList = db.Quadrantes.ToList();
                editViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.ToList();
                editViewModel.PerguntasList = db.Perguntas.ToList();

                return View(editViewModel);
            }
        }

        public ActionResult Delete(int id)
        {
            TecnicasViewModel deleteViewModel = new TecnicasViewModel();

            deleteViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == id));
            deleteViewModel.QuadrantesTecnicaList.AddRange(db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == id));

            foreach(Quadrante_Tecnica qt in deleteViewModel.QuadrantesTecnicaList)
            {
                deleteViewModel.QuadrantesList.Add(db.Quadrantes.Single(q => q.idQuadrante == qt.idQuadrante));
            }

            return View(deleteViewModel);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection dummy, int id)
        {
            db.Quadrante_Tecnicas.DeleteAllOnSubmit(db.Quadrante_Tecnicas.Where(qt => qt.idTecnica == id));
            db.Tecnicas.DeleteOnSubmit(db.Tecnicas.Single(t => t.idTecnica == id));
            db.SubmitChanges();

            return RedirectToAction("Index"); 
        }
        #endregion

        #region Quadrante
        public ActionResult DetailsQuadrante(int id)
        {
            TecnicasViewModel detailsQuadranteViewModel = new TecnicasViewModel();

            detailsQuadranteViewModel.QuadrantesTecnicaList = db.Quadrante_Tecnicas.Where(qt => qt.idQuadrante == id).ToList();

            foreach (Quadrante_Tecnica qt in detailsQuadranteViewModel.QuadrantesTecnicaList)
            {
                detailsQuadranteViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == qt.idTecnica));
            }

            detailsQuadranteViewModel.QuadrantesList.Add(db.Quadrantes.Single(q => q.idQuadrante == id));
            detailsQuadranteViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id).ToList();

            foreach (Pergunta_Quadrante pq in detailsQuadranteViewModel.PerguntasQuadrantesList)
            {
                detailsQuadranteViewModel.PerguntasList.Add(db.Perguntas.Single(p => p.idPergunta == pq.idPergunta));
            }

            return View(detailsQuadranteViewModel);
        }

        public ActionResult CreateQuadrante()
        {
            TecnicasViewModel createQuadranteViewModel = new TecnicasViewModel();

            createQuadranteViewModel.QuadrantesList.Add(new Quadrante());
            createQuadranteViewModel.PerguntasList = db.Perguntas.ToList();

            return View(createQuadranteViewModel);
        }

        [HttpPost]
        public ActionResult CreateQuadrante(FormCollection dadosQuadrante)
        {
            if(string.IsNullOrEmpty(dadosQuadrante["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome do quadrante!");
            }

            if(ModelState.IsValid == true)
            {
                Quadrante newQuadrante = new Quadrante();

                newQuadrante.nome = dadosQuadrante["nome"];
                newQuadrante.idUtilizador = Convert.ToInt32(Session["idUtilizadorAutenticado"]);

                db.Quadrantes.InsertOnSubmit(newQuadrante);
                db.SubmitChanges();

                foreach (Pergunta p in db.Perguntas)
                {
                    //Visto que o ASP.NET MVC faz umas coisas esquisitas com as checkboxes, este workaround certifica que a checkbox de cada pergunta é corretamente validada
                    if (dadosQuadrante[string.Concat("pergunta", p.idPergunta)].Contains("true") == true)
                    {
                        Pergunta_Quadrante newPerguntaQuadrante = new Pergunta_Quadrante();

                        newPerguntaQuadrante.idPergunta = p.idPergunta;
                        newPerguntaQuadrante.idQuadrante = newQuadrante.idQuadrante;

                        db.Pergunta_Quadrantes.InsertOnSubmit(newPerguntaQuadrante);
                    }
                }

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TecnicasViewModel createQuadranteViewModel = new TecnicasViewModel();

                createQuadranteViewModel.QuadrantesList.Add(new Quadrante());
                createQuadranteViewModel.PerguntasList = db.Perguntas.ToList();

                return View(createQuadranteViewModel);
            }
        }

        public ActionResult EditQuadrante(int id)
        {
            TecnicasViewModel editQuadranteViewModel = new TecnicasViewModel();

            editQuadranteViewModel.QuadrantesList.Add(db.Quadrantes.Single(q => q.idQuadrante == id));
            editQuadranteViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id).ToList();
            editQuadranteViewModel.PerguntasList = db.Perguntas.ToList();

            return View(editQuadranteViewModel);
        }

        [HttpPost]
        public ActionResult EditQuadrante(FormCollection dadosNovos, int id)
        {
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc.)
            if (string.IsNullOrEmpty(dadosNovos["nome"]) == true)
            {
                ModelState.AddModelError("nome", "Tem que preencher o campo do nome do quadrante!");
            }

            if(ModelState.IsValid == true)
            {
                //Primeiro obtém o quadrante a editar, através do seu resptivo ID passado como parâmetro
                //Depois associa os novos dados a esse mesmo quadrante
                Quadrante editQuadrante = db.Quadrantes.Single(q => q.idQuadrante == id);

                editQuadrante.nome = dadosNovos["nome"];

                //Apaga todas as relações pergunta-quadrante associadas a este quadrante particular
                db.Pergunta_Quadrantes.DeleteAllOnSubmit(db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id));
                db.SubmitChanges();

                //Depois das relações apagadas, volta a criar novas relações pergunta-quadrante, com base nas novas checkboxes seleccionadas
                foreach (Pergunta p in db.Perguntas)
                {
                    //Visto que o ASP.NET MVC faz umas coisas esquisitas com as checkboxes, este workaround certifica que a checkbox de cada pergunta é corretamente validada
                    if (dadosNovos[string.Concat("pergunta", p.idPergunta)].Contains("true") == true)
                    {
                        Pergunta_Quadrante newPerguntaQuadrante = new Pergunta_Quadrante();

                        newPerguntaQuadrante.idPergunta = p.idPergunta;
                        newPerguntaQuadrante.idQuadrante = editQuadrante.idQuadrante;

                        db.Pergunta_Quadrantes.InsertOnSubmit(newPerguntaQuadrante);
                    }
                }

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TecnicasViewModel editQuadranteViewModel = new TecnicasViewModel();

                editQuadranteViewModel.QuadrantesList.Add(db.Quadrantes.Single(q => q.idQuadrante == id));
                editQuadranteViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id).ToList();
                editQuadranteViewModel.PerguntasList = db.Perguntas.ToList();

                return View(editQuadranteViewModel);
            }
        }

        public ActionResult DeleteQuadrante(int id)
        {
            TecnicasViewModel deleteQuadranteViewModel = new TecnicasViewModel();

            deleteQuadranteViewModel.QuadrantesTecnicaList = db.Quadrante_Tecnicas.Where(qt => qt.idQuadrante == id).ToList();

            foreach(Quadrante_Tecnica qt in deleteQuadranteViewModel.QuadrantesTecnicaList)
            {
                deleteQuadranteViewModel.TecnicasList.Add(db.Tecnicas.Single(t => t.idTecnica == qt.idTecnica));
            }

            deleteQuadranteViewModel.QuadrantesList.Add(db.Quadrantes.Single(q => q.idQuadrante == id));
            deleteQuadranteViewModel.PerguntasQuadrantesList = db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id).ToList();

            foreach(Pergunta_Quadrante pq in deleteQuadranteViewModel.PerguntasQuadrantesList)
            {
                deleteQuadranteViewModel.PerguntasList.Add(db.Perguntas.Single(p => p.idPergunta == pq.idPergunta));
            }

            return View(deleteQuadranteViewModel);
        }

        [HttpPost]
        public ActionResult DeleteQuadrante(FormCollection dummy, int id)
        {
            //Primeiro eliminamos todas as relações quadrante-técnica e pergunta-quadrante correspondentes a este quadrante
            //Só depois é que eliminamos o próprio quadrante da base de dados
            db.Quadrante_Tecnicas.DeleteAllOnSubmit(db.Quadrante_Tecnicas.Where(qt => qt.idQuadrante == id));
            db.Pergunta_Quadrantes.DeleteAllOnSubmit(db.Pergunta_Quadrantes.Where(pq => pq.idQuadrante == id));
            db.Quadrantes.DeleteOnSubmit(db.Quadrantes.Single(q => q.idQuadrante == id));
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddQuadrante(int idTecnica, int idQuadrante)
        {
            Quadrante_Tecnica newQuadranteTecnica = new Quadrante_Tecnica();

            newQuadranteTecnica.idTecnica = idTecnica;
            newQuadranteTecnica.idQuadrante = idQuadrante;

            db.Quadrante_Tecnicas.InsertOnSubmit(newQuadranteTecnica);
            db.SubmitChanges();

            return RedirectToAction("Edit", new { id = idTecnica });
        }

        [HttpPost]
        public ActionResult RemoveQuadrante(int id)
        {
            //Neste caso, como só queremos remover o quadrante da técnica (e não eliminá-lo da base de dados), obtemos a relação quadrante-técnica correspondente
            //Depois de obtida, eliminamos essa mesma relação da base de dados
            Quadrante_Tecnica removeQuadranteTecnica = db.Quadrante_Tecnicas.Single(qt => qt.idQuadrante_Tecnica == id);

            int removeQuadranteTecnicaIDTecnica = removeQuadranteTecnica.idTecnica;

            db.Quadrante_Tecnicas.DeleteOnSubmit(removeQuadranteTecnica);
            db.SubmitChanges();

            return RedirectToAction("Edit", new { id = removeQuadranteTecnicaIDTecnica });
        }
        #endregion

        #region Pergunta
        public ActionResult CreatePergunta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePergunta(FormCollection dadosNovos)
        {
            if(string.IsNullOrEmpty(dadosNovos["texto"]) == true)
            {
                ModelState.AddModelError("texto", "Tem que introduzir um texto válido!");
            }

            if(string.IsNullOrEmpty(dadosNovos["importancia"]) == true)
            {
                ModelState.AddModelError("importancia", "Tem que introduzir uma importância válida para esta pergunta!");
            }

            if(ModelState.IsValid == true)
            {
                Pergunta newPergunta = new Pergunta();

                newPergunta.texto = dadosNovos["texto"];
                newPergunta.importancia = Convert.ToInt32(dadosNovos["importancia"]);
                newPergunta.idUtilizador = Convert.ToInt32(Session["idUtilizadorAutenticado"]);

                db.Perguntas.InsertOnSubmit(newPergunta);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditPergunta(int id)
        {
            return View(db.Perguntas.Single(p => p.idPergunta == id));
        }

        [HttpPost]
        public ActionResult EditPergunta(FormCollection dadosNovos, int id)
        {
            if(string.IsNullOrEmpty(dadosNovos["texto"]) == true)
            {
                ModelState.AddModelError("texto", "Tem que introduzir um texto válido!");
            }

            if(string.IsNullOrEmpty(dadosNovos["importancia"]) == true)
            {
                ModelState.AddModelError("importancia", "Tem que introduzir uma importância válida para esta pergunta!");
            }

            if(ModelState.IsValid == true)
            {
                Pergunta editPergunta = db.Perguntas.Single(p => p.idPergunta == id);

                editPergunta.texto = dadosNovos["texto"];
                editPergunta.importancia = Convert.ToInt32(dadosNovos["importancia"]);

                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeletePergunta(int id)
        {
            TecnicasViewModel deletePerguntaViewModel = new TecnicasViewModel();

            deletePerguntaViewModel.PerguntasList.Add(db.Perguntas.Single(p => p.idPergunta == id));

            return View(deletePerguntaViewModel);
        }

        [HttpPost]
        public ActionResult DeletePergunta(FormCollection dummy, int id)
        {
            //Primeiro eliminamos todas as relações pergunta-quadrante correspondentes a esta pergunta
            //Só depois é que eliminamos a própria pergunta da base de dados
            db.Pergunta_Quadrantes.DeleteAllOnSubmit(db.Pergunta_Quadrantes.Where(pq => pq.idPergunta == id));
            db.Perguntas.DeleteOnSubmit(db.Perguntas.Single(p => p.idPergunta == id));
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AddPergunta(int idQuadrante, int idPergunta)
        {
            Pergunta_Quadrante newPerguntaQuadrante = new Pergunta_Quadrante();

            newPerguntaQuadrante.idQuadrante = idQuadrante;
            newPerguntaQuadrante.idPergunta = idPergunta;

            db.Pergunta_Quadrantes.InsertOnSubmit(newPerguntaQuadrante);
            db.SubmitChanges();

            return RedirectToAction("EditQuadrante", new { id = idQuadrante });
        }

        public ActionResult RemovePergunta(int id)
        {
            //Neste caso, como só queremos remover a pergunta do quadrante (e não eliminá-la da base de dados), obtemos a relação pergunta-quadrante correspondente
            //Depois de obtida, eliminamos essa mesma relação da base de dados
            Pergunta_Quadrante removePerguntaQuadrante = db.Pergunta_Quadrantes.Single(pq => pq.idPergunta_Quadrante == id);

            int removePerguntaQuadranteIDQuadrante = removePerguntaQuadrante.idQuadrante;

            db.Pergunta_Quadrantes.DeleteOnSubmit(removePerguntaQuadrante);
            db.SubmitChanges();

            return RedirectToAction("EditQuadrante", new { id = removePerguntaQuadranteIDQuadrante });
        }
        #endregion
    }
}