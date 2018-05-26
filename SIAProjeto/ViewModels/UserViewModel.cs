using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SIAProjeto.Models;

namespace SIAProjeto.ViewModels
{
    public class UserViewModel
    {
        private Utilizador utilizador;
        private List<Teste> testesList;

        public Utilizador Utilizador
        {
            get
            {
                return utilizador;
            }

            set
            {
                utilizador = value;
            }
        }

        public List<Teste> TestesList
        {
            get
            {
                return testesList;
            }

            set
            {
                testesList = value;
            }
        }

        public UserViewModel()
        {
            utilizador = new Utilizador();
            testesList = new List<Teste>();
        }
    }
}