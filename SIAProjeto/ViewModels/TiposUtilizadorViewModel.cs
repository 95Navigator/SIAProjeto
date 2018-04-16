using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SIAProjeto.Models;

namespace SIAProjeto.ViewModels
{
    public class TiposUtilizadorViewModel
    {
        private TipoUtilizador tipoUtilizador;
        private List<Permissao_TipoUtilizador> permissoesTipoUtilizadorList;
        private List<Permissao> permissoesList;

        public TipoUtilizador TipoUtilizador
        {
            get
            {
                return tipoUtilizador;
            }

            set
            {
                tipoUtilizador = value;
            }
        }

        public List<Permissao_TipoUtilizador> PermissoesTipoUtilizadorList
        {
            get
            {
                return permissoesTipoUtilizadorList;
            }

            set
            {
                permissoesTipoUtilizadorList = value;
            }
        }

        public List<Permissao> PermissoesList
        {
            get
            {
                return permissoesList;
            }

            set
            {
                permissoesList = value;
            }
        }

        public TiposUtilizadorViewModel()
        {
            tipoUtilizador = new TipoUtilizador();
            permissoesTipoUtilizadorList = new List<Permissao_TipoUtilizador>();
            permissoesList = new List<Permissao>();
        }
    }
}