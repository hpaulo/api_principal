using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbDispositivoUsuario
    {
        public int idDispositivoUsuario { get; set; }
        public int idUser { get; set; }
        public string dsPlataforma { get; set; }
        public string dsModelo { get; set; }
        public string dsVersao { get; set; }
        public string idIONIC { get; set; }
        public string idUserIONIC { get; set; }
        public string cdTokenValido { get; set; }
        public Nullable<short> tmLargura { get; set; }
        public Nullable<short> tmAltura { get; set; }
    }
}