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
            Tecnica novaT = new Tecnica();
            Pergunta novaP = new Pergunta();
            Quadrante novoQ = new Quadrante(); 

            return View();
        }

        [HttpPost] // Criar nova Técnica 
        public ActionResult CriarTecnica(Tecnica tecnica)
        {
                        

            return null; 
        }
        private void Inst()
        {
            
        }


    }
}