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
        private DataClassesDBMainDataContext db;

        public HomeController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            if(Session["loginTentativas"] == null)
            {
                Session["loginTentativas"] = 5;
            }

            if(Session["loginDateTimeBloqueio"] != null)
            {
                if(Convert.ToDateTime(Session["loginDateTimeBloqueio"]))
                {
                    
                }
                else
                {

                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection dadosLogin)
        {
            if(Convert.ToInt32(Session["loginTentativas"]) > 0)
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
                    //Depois guarda também o ID do tipo desse utilizador
                    //Por fim atualiza o estado de autenticação e a data da última autenticação desse utilizador na base de dados
                    if (auxUtilizador != default(Utilizador))
                    {
                        Session["idUtilizadorAutenticado"] = auxUtilizador.idUtilizador;

                        auxUtilizador.estadoAutenticacao = true;
                        auxUtilizador.dataUltimaAutenticacao = DateTime.Now;

                        db.SubmitChanges();

                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        //Se não for encontrada nenhuma correspondência, significa que o utilizador enganou-se nas credenciais e o login falhou
                        //Logo retira uma tentativa do contador de tentativas de login falhadas
                        Session["loginTentativas"] = Convert.ToInt32(Session["loginTentativas"]) - 1;

                        ModelState.AddModelError("email", "Introduziu uma combinação incorreta! Possui agora " + Session["loginTentativas"] + " tentativas de autenticação restantes.");
                    }
                }
            }
            else
            {
                //Se o utilizador atingiu as cinco tentativas de login falhadas, bloqueia as caixas de texto por cinco minutos
                Session["loginDateTimeBloqueio"] = DateTime.Now;

                ModelState.AddModelError("email", "Esgotou o número de tentativas de autenticação! Encontra-se agora bloqueado do sistema.");
            }

            return View();
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
            else
            {
                if(db.Utilizadors.SingleOrDefault(u => u.email == dadosRegisto["email"]) != default(Utilizador))
                {
                    ModelState.AddModelError("email", "Um utilizador com este e-mail já se encontra registado!");
                }
            }

            if(string.IsNullOrEmpty(dadosRegisto["password"]) == true)
            {
                ModelState.AddModelError("password", "Tem que preencher o campo da palavra-passe!");
            }
            else
            {
                if(dadosRegisto["password"].Length < 8)
                {
                    ModelState.AddModelError("password", "A palavra-passe introduzida tem que possuir mais de 8 caracteres!");
                }
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
                newUtilizador.idTipoUtilizador = 1;

                db.Utilizadors.InsertOnSubmit(newUtilizador);

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