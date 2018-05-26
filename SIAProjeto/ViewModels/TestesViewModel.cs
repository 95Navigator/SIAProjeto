using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SIAProjeto.Models;

namespace SIAProjeto.ViewModels
{
    public class TestesViewModel
    {
        private Teste teste;
        private Tecnica tecnica;
        private Quadrante quadrante;
        private List<Pergunta> perguntasList;

        private string[] perguntaImportancias;

        public Teste Teste
        {
            get
            {
                return teste;
            }

            set
            {
                teste = value;
            }
        }

        public Tecnica Tecnica
        {
            get
            {
                return tecnica;
            }

            set
            {
                tecnica = value;
            }
        }

        public Quadrante Quadrante
        {
            get
            {
                return quadrante;
            }

            set
            {
                quadrante = value;
            }
        }

        public List<Pergunta> PerguntasList
        {
            get
            {
                return perguntasList;
            }

            set
            {
                perguntasList = value;
            }
        }

        public string[] PerguntaImportancias
        {
            get
            {
                return perguntaImportancias;
            }

            set
            {
                perguntaImportancias = value;
            }
        }

        public TestesViewModel()
        {
            teste = new Teste();
            tecnica = new Tecnica();
            quadrante = new Quadrante();
            perguntasList = new List<Pergunta>();

            perguntaImportancias = new string[] { "Totalmente sem importância", "Pouca importância", "Importante", "Muito importante", "Totalmente importante" };
        }
    }
}