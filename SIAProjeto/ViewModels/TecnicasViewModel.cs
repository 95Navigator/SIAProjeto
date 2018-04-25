using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SIAProjeto.Models;

namespace SIAProjeto.ViewModels
{
    public class TecnicasViewModel
    {
        private Tecnica tecnica;
        private List<Quadrante_Tecnica> quadrantesTecnicaList;
        private List<Quadrante> quadrantesList;
        private List<Pergunta_Quadrante> perguntasQuadrantesList;
        private List<Pergunta> perguntasList;

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

        public List<Quadrante_Tecnica> QuadrantesTecnicaList
        {
            get
            {
                return quadrantesTecnicaList;
            }

            set
            {
                quadrantesTecnicaList = value;
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
            tecnica = new Tecnica();
            quadrantesTecnicaList = new List<Quadrante_Tecnica>();
            quadrantesList = new List<Quadrante>();
            perguntasQuadrantesList = new List<Pergunta_Quadrante>();
            perguntasList = new List<Pergunta>();
        }
    }
}