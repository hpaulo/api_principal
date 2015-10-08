namespace api.Models
{
    public partial class tbRecebimentoAjuste
    {
        public int idRecebimentoAjuste { get; set; }
        public System.DateTime dtAjuste { get; set; }
        public string nrCNPJ { get; set; }
        public int cdBandeira { get; set; }
        public string dsMotivo { get; set; }
        public decimal vlAjuste { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
    }
}
