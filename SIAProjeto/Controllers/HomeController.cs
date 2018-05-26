using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net.Mail;

using SIAProjeto.Models;

namespace SIAProjeto.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private DataClassesDBMainDataContext db;

        public HomeController()
        {
            db = new DataClassesDBMainDataContext();
        }

        public ActionResult Index()
        {
            ViewBag.isUtilizadorBloqueado = false;

            //Define na sessão, se ainda não estiver definido, o número possível de tentativas falhadas de autenticação
            if (Session["loginTentativas"] == null)
            {
                Session["loginTentativas"] = 5;
            }

            //Se esta variável de sessão já se encontrar definida, significa que o utilizador foi bloquado do sistema de autenticação e que os cinco minutos já estão a contar
            if(Session["loginDateTimeBloqueio"] != null)
            {
                //Obtém a diferença entre o momento atual e o momento em que o sistema de autenticação foi bloqueado
                TimeSpan loginTimeSpanBloqueio = DateTime.Now.Subtract(Convert.ToDateTime(Session["loginDateTimeBloqueio"]));

                //Se essa diferença for inferior a cinco minutos, então o sistema de autenticação deve continuar bloqueado
                //Se essa diferença for igual ou exceder os cinco minutos, então o sistema de autenticação já pode ser desbloqueado
                if (loginTimeSpanBloqueio.Minutes < 5)
                {
                    ViewBag.isUtilizadorBloqueado = true;
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection dadosLogin)
        {
            //Enquanto o utilizador não ultrapassar as suas tentativas, a autenticação é permitida
            if(Convert.ToInt32(Session["loginTentativas"]) != 1)
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

                        //Define o timeout da sessão para 120 minutos
                        Session.Timeout = 120;

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

                ViewBag.isUtilizadorBloqueado = true;
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
            //Verifica cada dado introduzido pelo utilizador por inconsistências (se os campos estão preenchidos, se os campos são válidos, etc...)
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

                //Envia um e-mail de confirmação para o e-mail do novo Utilizador
                SmtpClient confirmEmailClient = new SmtpClient("smtp.gmail.com");

                MailMessage confirmEmail = new MailMessage();

                //Configura o cliente SMTP do Gmail
                confirmEmailClient.Port = 587;
                confirmEmailClient.UseDefaultCredentials = false;
                confirmEmailClient.Credentials = new System.Net.NetworkCredential("", "");
                confirmEmailClient.EnableSsl = true;

                confirmEmail.From = new MailAddress("", "");
                confirmEmail.To.Add(new MailAddress(newUtilizador.email));
                confirmEmail.Subject = "E-mail de confirmação de novo Utilizador";
                confirmEmail.IsBodyHtml = true;

                confirmEmail.Body = System.IO.File.ReadAllText(Server.MapPath(@"\Assets\ActivateUserMailHtml.html"));
                confirmEmail.Body = confirmEmail.Body.Replace("@id", newUtilizador.idUtilizador.ToString());

                confirmEmailClient.Send(confirmEmail);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult AtivarRegisto(int id)
        {
            //Ativa a conta do novo utilizador, mudando o seu estado de ativação para true
            Utilizador newUtilizador = db.Utilizadors.Single(u => u.idUtilizador == id);

            newUtilizador.estadoAtivacao = true;

            //Submete esta mudança na base de dados
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarPassword(FormCollection dadosRecuperacao)
        {
            return View();
        }
    }
}