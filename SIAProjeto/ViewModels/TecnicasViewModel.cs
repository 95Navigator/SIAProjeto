using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SIAProjeto.Models;

namespace SIAProjeto.ViewModels
{
    public class TecnicasViewModel
    {
        private List<Tecnica> tecnicasList;
        private List<Quadrante> quadrantesList;
        private List<Pergunta_Quadrante> perguntasQuadrantesList;
        private List<Pergunta> perguntasList;

        public List<Tecnica> TecnicasList
        {
            get
            {
                return tecnicasList;
            }

            set
            {
                tecnicasList = value;
            }
        }

        public List<Quadrante> QuadrantesList
        {
            get
            {
                return quadrantesList;
            }

            set
            {
                quadrantesList = value;
            }
        }

        public List<Pergunta_Quadrante> PerguntasQuadrantesList
        {
            get
            {
                return perguntasQuadrantesList;
            }

            set
            {
                perguntasQuadrantesList = value;
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

        public TecnicasViewModel()
        {
            tecnicasList = new List<Tecnica>();
            quadrantesList = new List<Quadrante>();
            perguntasQuadrantesList = new List<Pergunta_Quadrante>();
            perguntasList = new List<Pergunta>();
        }
    }
}