using api.Bibliotecas;
using api.Models.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Negocios.Util
{
    public class GatewayBancos
    {
        static List<CodigoCompensacaoBancos> ListBancos = new List<CodigoCompensacaoBancos>();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBancos()
        {
            //Populate();
        }

        public static void Populate()
        {
            #region Add Itens in List
                if (ListBancos.Count == 0)
                {
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "117", NomeReduzido = "SC Advanced", NomeExtenso = "Advanced Corretora de Câmbio Ltda." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "123", NomeReduzido = "SCFI Agiplan", NomeExtenso = "Agiplan Financeira S.A. - Crédito, Financiamento e Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "654", NomeReduzido = "A. J. Renner", NomeExtenso = "Banco A. J. Renner S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "246", NomeReduzido = "ABC - Brasil", NomeExtenso = "Banco ABC Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "075", NomeReduzido = "ABN Amro", NomeExtenso = "Banco ABN Amro S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "025", NomeReduzido = "Alfa", NomeExtenso = "Banco Alfa S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "641", NomeReduzido = "Alvorada", NomeExtenso = "Banco Alvorada S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "213", NomeReduzido = "Arbi", NomeExtenso = "Banco Arbi S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "019", NomeReduzido = "Azteca", NomeExtenso = "Banco Azteca do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "024", NomeReduzido = "Bandepe", NomeExtenso = "Banco Bandepe S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "740", NomeReduzido = "Barclays", NomeExtenso = "Banco Barclays S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "107", NomeReduzido = "BBM", NomeExtenso = "Banco BBM S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "739", NomeReduzido = "BGN", NomeExtenso = "Banco BGN S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "096", NomeReduzido = "Banco BMFBovespa", NomeExtenso = "Banco BMFBovespa de Serviços de Liquidação e Custódia S/A" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "318", NomeReduzido = "BMG", NomeExtenso = "Banco BMG S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "752", NomeReduzido = "BNP Paribas Brasil", NomeExtenso = "Banco BNP Paribas Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "248", NomeReduzido = "Boavista Interatlântico", NomeExtenso = "Banco Boavista Interatlântico S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "218", NomeReduzido = "Bonsucesso", NomeExtenso = "Banco Bonsucesso S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "065", NomeReduzido = "Bracce", NomeExtenso = "Banco Bracce S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "063", NomeReduzido = "Bradescard", NomeExtenso = "Banco Bradescard S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "036", NomeReduzido = "Bradesco BBI", NomeExtenso = "Banco Bradesco BBI S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "122", NomeReduzido = "Bradesco BERJ", NomeExtenso = "Banco Bradesco BERJ S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "204", NomeReduzido = "Bradesco Cartões", NomeExtenso = "Banco Bradesco Cartões S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "394", NomeReduzido = "Bradesco Financiamentos", NomeExtenso = "Banco Bradesco Financiamentos S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "237", NomeReduzido = "Bradesco", NomeExtenso = "Banco Bradesco S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "208", NomeReduzido = "BTG Pactual", NomeExtenso = "Banco BTG Pactual S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "263", NomeReduzido = "Cacique", NomeExtenso = "Banco Cacique S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "473", NomeReduzido = "Caixa Geral - Brasil", NomeExtenso = "Banco Caixa Geral - Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "412", NomeReduzido = "Capital", NomeExtenso = "Banco Capital S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "040", NomeReduzido = "Cargill", NomeExtenso = "Banco Cargill S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "266", NomeReduzido = "Cédula", NomeExtenso = "Banco Cédula S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "233", NomeReduzido = "Cifra", NomeExtenso = "Banco Cifra S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "745", NomeReduzido = "Citibank S. A.", NomeExtenso = "Banco Citibank S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "241", NomeReduzido = "Clássico", NomeExtenso = "Banco Clássico S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "095", NomeReduzido = "BCam Confidence", NomeExtenso = "Banco Confidence de Câmbio S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "756", NomeReduzido = "Cooperativo Brasil", NomeExtenso = "Banco Cooperativo do Brasil S/A - Bancoob" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "748", NomeReduzido = "Sicredi", NomeExtenso = "Banco Cooperativo Sicredi S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "222", NomeReduzido = "Credit Agrícole Brasil", NomeExtenso = "Banco Credit Agrícole Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "505", NomeReduzido = "Credit Suisse", NomeExtenso = "Banco Credit Suisse (Brasil) S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "003", NomeReduzido = "Amazônia - BASA", NomeExtenso = "Banco da Amazônia S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "083", NomeReduzido = "China Brasil", NomeExtenso = "Banco da China Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "707", NomeReduzido = "Daycoval", NomeExtenso = "Banco Daycoval S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "070", NomeReduzido = "BRB - Bco. Brasília", NomeExtenso = "Banco de Brasília S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "300", NomeReduzido = "Nacion Argentina", NomeExtenso = "Banco de la Nacion Argentina" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "495", NomeReduzido = "Província B. Aires", NomeExtenso = "Banco de La Provincia de Buenos Aires" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "494", NomeReduzido = "Rep. Or. Uruguay", NomeExtenso = "Banco de La Republica Oriental del Uruguay" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "456", NomeReduzido = "Tokyo - Mitsubishi UFJ", NomeExtenso = "Banco de Tokyo-Mitsubishi UFJ Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "214", NomeReduzido = "Dibens", NomeExtenso = "Banco Dibens S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "001", NomeReduzido = "Brasil", NomeExtenso = "Banco do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "047", NomeReduzido = "Est. SE - Banese", NomeExtenso = "Banco do Estado de Sergipe S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "037", NomeReduzido = "Est. PA - Banpará", NomeExtenso = "Banco do Estado do Pará S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "041", NomeReduzido = "Est. RS - Banrisul", NomeExtenso = "Banco do Estado do Rio Grande do Sul S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "004", NomeReduzido = "BNB", NomeExtenso = "Banco do Nordeste do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "265", NomeReduzido = "Fator", NomeExtenso = "Banco Fator S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "224", NomeReduzido = "Fibra", NomeExtenso = "Banco Fibra S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "626", NomeReduzido = "Ficsa", NomeExtenso = "Banco Ficsa S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "121", NomeReduzido = "Gerador", NomeExtenso = "Banco Gerador S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "612", NomeReduzido = "Guanabara", NomeExtenso = "Banco Guanabara S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "604", NomeReduzido = "Indl. do Brasil", NomeExtenso = "Banco Industrial do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "320", NomeReduzido = "Indl. e Coml.", NomeExtenso = "Banco Industrial e Comercial S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "653", NomeReduzido = "Indusval", NomeExtenso = "Banco Indusval S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "630", NomeReduzido = "Intercap", NomeExtenso = "Banco Intercap S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "077", NomeReduzido = "Intermedium", NomeExtenso = "Banco Intermedium S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "249", NomeReduzido = "Investcred", NomeExtenso = "Banco Investcred Unibanco S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "184", NomeReduzido = "Itaú BBA", NomeExtenso = "Banco Itaú BBA S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "029", NomeReduzido = "Itaú BMG Consignado", NomeExtenso = "Banco Itaú BMG Consignado S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "479", NomeReduzido = "ItauBank", NomeExtenso = "Banco ItauBank S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "376", NomeReduzido = "J. P. Morgan", NomeExtenso = "Banco J. P. Morgan S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "074", NomeReduzido = "J. Safra", NomeExtenso = "Banco J. Safra S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "217", NomeReduzido = "John Deere", NomeExtenso = "Banco John Deere S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "076", NomeReduzido = "KDB do Brasil", NomeExtenso = "Banco KDB do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "757", NomeReduzido = "Keb do Brasil", NomeExtenso = "Banco Keb do Brasil S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "600", NomeReduzido = "Luso Brasileiro", NomeExtenso = "Banco Luso Brasileiro S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "243", NomeReduzido = "Máxima", NomeExtenso = "Banco Máxima S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "389", NomeReduzido = "Mercantil do Brasil", NomeExtenso = "Banco Mercantil do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "370", NomeReduzido = "Mizuho", NomeExtenso = "Banco Mizuho do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "746", NomeReduzido = "Modal", NomeExtenso = "Banco Modal S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "066", NomeReduzido = "Morgan Stanley", NomeExtenso = "Banco Morgan Stanley S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "079", NomeReduzido = "Original do Agronegócio", NomeExtenso = "Banco Original do Agronegócio S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "212", NomeReduzido = "Original", NomeExtenso = "Banco Original S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "623", NomeReduzido = "Panamericano", NomeExtenso = "Banco Panamericano S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "611", NomeReduzido = "Paulista", NomeExtenso = "Banco Paulista S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "613", NomeReduzido = "Pecúnia", NomeExtenso = "Banco Pecúnia S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "094", NomeReduzido = "Petra", NomeExtenso = "Banco Petra S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "643", NomeReduzido = "Pine", NomeExtenso = "Banco Pine S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "735", NomeReduzido = "Pottencial", NomeExtenso = "Banco Pottencial S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "747", NomeReduzido = "Rabobank", NomeExtenso = "Banco Rabobank International Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "088", NomeReduzido = "Randon", NomeExtenso = "Banco Randon S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "633", NomeReduzido = "Rendimento", NomeExtenso = "Banco Rendimento S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "741", NomeReduzido = "Ribeirão Preto", NomeExtenso = "Banco Ribeirão Preto S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "120", NomeReduzido = "Rodobens", NomeExtenso = "Banco Rodobens SA" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "422", NomeReduzido = "Safra", NomeExtenso = "Banco Safra S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "033", NomeReduzido = "Santander", NomeExtenso = "Banco Santander (Brasil) S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "743", NomeReduzido = "Semear", NomeExtenso = "Banco Semear S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "366", NomeReduzido = "Société Générale", NomeExtenso = "Banco Société Générale Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "637", NomeReduzido = "Sofisa", NomeExtenso = "Banco Sofisa S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "012", NomeReduzido = "BI Standard", NomeExtenso = "Banco Standard de Investimentos S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "464", NomeReduzido = "Sumitomo Mitsui", NomeExtenso = "Banco Sumitomo Mitsui Brasileiro S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "082", NomeReduzido = "Topazio", NomeExtenso = "Banco Topazio S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "634", NomeReduzido = "Triângulo", NomeExtenso = "Banco Triângulo S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "018", NomeReduzido = "BM Tricury", NomeExtenso = "Banco Tricury S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "655", NomeReduzido = "Votorantim", NomeExtenso = "Banco Votorantim S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "610", NomeReduzido = "VR", NomeExtenso = "Banco VR S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "119", NomeReduzido = "Western Union", NomeExtenso = "Banco Western Union do Brasil SA" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "124", NomeReduzido = "Woori Bank", NomeExtenso = "Banco Woori Bank do Brasil S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "021", NomeReduzido = "Est. ES - Banestes", NomeExtenso = "Banestes S.A. Banco do Estado do Espírito Santo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "719", NomeReduzido = "Banif", NomeExtenso = "Banif - Bco Internacional do Funchal (Brasil) S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "755", NomeReduzido = "Bank of America Merrill Lynch", NomeExtenso = "Bank of America Merrill Lynch Banco Múltiplo S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "081", NomeReduzido = "BBN Banco Brasileiro de Negócios", NomeExtenso = "BBN Banco Brasileiro de Negocios S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "250", NomeReduzido = "BCV", NomeExtenso = "BCV - Banco de Crédito e Varejo S/A" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "078", NomeReduzido = "BI BES - Espirito Santo", NomeExtenso = "BES Investimento do Brasil S.A - Banco de Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "017", NomeReduzido = "BNY Mellon", NomeExtenso = "BNY Mellon Banco S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "069", NomeReduzido = "BPN Brasil", NomeExtenso = "BPN Brasil Banco Múltiplo S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "126", NomeReduzido = "BI BR Partners", NomeExtenso = "BR Partners Banco de Investimento S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "125", NomeReduzido = "Brasil Plural", NomeExtenso = "Brasil Plural S.A. Banco Múltiplo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "092", NomeReduzido = "SCFI Brickell", NomeExtenso = "Brickell S.A. Crédito, Financiamento e Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "080", NomeReduzido = "SC BT Associados", NomeExtenso = "BT Associados Corretora de Câmbio Ltda" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "104", NomeReduzido = "Caixa Econ. Federal", NomeExtenso = "Caixa Econômica Federal" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "130", NomeReduzido = "SCFI Caruana", NomeExtenso = "Caruana S.A. Sociedade de Crédito, Financiamento e Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "112", NomeReduzido = "CC Unicred Brasil Central", NomeExtenso = "Central das Cooperativas de Crédito do Brasil Central" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "114", NomeReduzido = "CC Cecoopes", NomeExtenso = "Central das Cooperativas de Economia e Crédito Mútuo do Estado do Espírito Santo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "091", NomeReduzido = "CC Unicred Central RS", NomeExtenso = "Central de Cooperativas de Economia e Crédito Mútuo do Est RS - Unicred" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "477", NomeReduzido = "Citibank N. A.", NomeExtenso = "Citibank N.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "127", NomeReduzido = "SC Codepe", NomeExtenso = "Codepe - Corretora de Valores S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "060", NomeReduzido = "SC Confidence", NomeExtenso = "Confidence Corretora de Câmbio S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "016", NomeReduzido = "CC Sicoob Creditran", NomeExtenso = "Coop de Créd. Mútuo dos Despachantes de Trânsito de SC e Região Metrop. de PA/RS" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "090", NomeReduzido = "CC Unicred Central SP", NomeExtenso = "Cooperativa Central de Crédito do Estado de São Paulo Ltda. - Unicred Central SP" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "097", NomeReduzido = "CC Centralcredi", NomeExtenso = "Cooperativa Central de Crédito Noroeste Brasileiro Ltda - CentralCredi" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "085", NomeReduzido = "CC Cecred", NomeExtenso = "Cooperativa Central de Crédito Urbano - Cecred" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "087", NomeReduzido = "CC Unicred Central Santa Catarina", NomeExtenso = "Cooperativa Central de Economia e Crédito Mútuo das Unicreds do Estado de SC Ltd" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "089", NomeReduzido = "CC Região da Mogiana", NomeExtenso = "Cooperativa de Crédito Rural da Região da Mogiana" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "901", NomeReduzido = "SC Souza Barros", NomeExtenso = "Corretora Souza Barros Câmbio e Títulos S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "098", NomeReduzido = "CC Credialiança", NomeExtenso = "Credialiança Cooperativa de Crédito Rural" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "010", NomeReduzido = "CC Credicoamo", NomeExtenso = "Credicoamo Crédito Rural Cooperativa" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "011", NomeReduzido = "SC Credit Suisse HG", NomeExtenso = "Credit Suisse Hedging-Griffo Corretora de Valores S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "487", NomeReduzido = "Deutsche Bank", NomeExtenso = "Deutsche Bank S.A. - Banco Alemão" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "064", NomeReduzido = "BM Goldman Sachs", NomeExtenso = "Goldman Sachs do Brasil  Banco Múltiplo S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "062", NomeReduzido = "Hipercard", NomeExtenso = "Hipercard Banco Múltiplo S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "399", NomeReduzido = "HSBC Bank", NomeExtenso = "HSBC Bank Brasil S. A. - Banco Multiplo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "132", NomeReduzido = "ICBC do Brasil", NomeExtenso = "ICBC do Brasil Banco Múltiplo S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "492", NomeReduzido = "ING Bank", NomeExtenso = "ING Bank N.V." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "652", NomeReduzido = "Itaú Unibanco Holding", NomeExtenso = "Itaú Unibanco Holding S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "341", NomeReduzido = "Itaú Unibanco", NomeExtenso = "Itaú Unibanco S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "488", NomeReduzido = "JP Morgan Chase Bank", NomeExtenso = "JPMorgan Chase Bank, National Association" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "105", NomeReduzido = "SCFI LECCA", NomeExtenso = "Lecca Crédito, Financiamento e Investimento S/A" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "113", NomeReduzido = "SC Magliano", NomeExtenso = "Magliano S.A. Corretora de Cambio e Valores Mobiliarios" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "014", NomeReduzido = "Natixis Brasil", NomeExtenso = "Natixis Brasil S.A. Banco Múltiplo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "753", NomeReduzido = "NBC Bank", NomeExtenso = "NBC Bank Brasil S. A. - Banco Múltiplo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "111", NomeReduzido = "DTVM Oliveira Trust", NomeExtenso = "Oliveira Trust Distribuidora de Títulos e Valores Mobiliários S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "254", NomeReduzido = "Paraná", NomeExtenso = "Parana Banco S. A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "100", NomeReduzido = "SC Planner", NomeExtenso = "Planner Corretora de Valores S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "093", NomeReduzido = "SCM Pólocred", NomeExtenso = "Pólocred Sociedade de Crédito ao Microempreendedor e à Empresa de Pequeno Porte" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "108", NomeReduzido = "SCFI Portocred", NomeExtenso = "PortoCred S.A. Crédito, Financiamento e Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "101", NomeReduzido = "DTVM Renascença", NomeExtenso = "Renascença Distribuidora de Títulos e Valores Mobiliários Ltda." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "751", NomeReduzido = "Scotiabank Brasil", NomeExtenso = "Scotiabank Brasil S.A. Banco Múltiplo" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "013", NomeReduzido = "SC Senso", NomeExtenso = "Senso Corretora de Câmbio e Valores Mobiliários S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "118", NomeReduzido = "BI Standard Chartered", NomeExtenso = "Standard Chartered Bank (Brasil) S.A. Banco de Investimento" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "131", NomeReduzido = "SC Tullett Prebon", NomeExtenso = "Tullett Prebon Brasil S.A. Corretora de Valores e Câmbio" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "015", NomeReduzido = "SC UBS Brasil", NomeExtenso = "UBS Brasil Corretora de Câmbio, Títulos e Valores Mobiliários S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "409", NomeReduzido = "Unibanco", NomeExtenso = "Unibanco-União de Bancos Brasileiros S.A." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "230", NomeReduzido = "Unicard", NomeExtenso = "Unicard Banco Múltiplo S.A" });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "099", NomeReduzido = "CC Uniprime Central", NomeExtenso = "Uniprime Central – Central Interestadual de Cooperativas de Crédito Ltda." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "084", NomeReduzido = "CC Uniprime Norte do Paraná", NomeExtenso = "Uniprime Norte do Paraná - Coop. de Econ e Crédito Mútuo dos Médicos." });
                    ListBancos.Add(new CodigoCompensacaoBancos { Codigo = "102", NomeReduzido = "SC XP Investimentos", NomeExtenso = "XP Investimentos Corretora de Câmbio Títulos e Valores Mobiliários S.A." });
                }
            #endregion
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CODIGO = 100,
            NOMEREDUZIDO = 101,
            NOMEEXTENSO = 102,
        };

        /// <summary>
        /// Get Banco
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<CodigoCompensacaoBancos> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = ListBancos.Select(e => new CodigoCompensacaoBancos { Codigo = e.Codigo, NomeReduzido = e.NomeReduzido, NomeExtenso = e.NomeExtenso }).AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CODIGO:
                        string codigo = item.Value.ToString();
                        entity = entity.Where(e => e.Codigo.Equals(codigo)).AsQueryable();
                        break;
                    case CAMPOS.NOMEREDUZIDO:
                        string nomereduzido = item.Value.ToString();
                        if (nomereduzido.Contains("%")) // usa LIKE
                        {
                            string busca = nomereduzido.Replace("%", "").ToString();
                            entity = entity.Where(e => e.NomeReduzido.Contains(busca)).AsQueryable();
                        }
                        else
                            entity = entity.Where(e => e.NomeReduzido.Equals(nomereduzido)).AsQueryable();
                        break;
                    case CAMPOS.NOMEEXTENSO:
                        string nomeextenso = item.Value.ToString();
                        if (nomeextenso.Contains("%")) // usa LIKE
                        {
                            string busca = nomeextenso.Replace("%", "").ToString();
                            entity = entity.Where(e => e.NomeExtenso.Contains(busca)).AsQueryable();
                        }
                        else
                            entity = entity.Where(e => e.NomeExtenso.Equals(nomeextenso)).AsQueryable();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CODIGO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Codigo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.Codigo).AsQueryable();
                    break;
                case CAMPOS.NOMEREDUZIDO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.NomeReduzido).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.NomeReduzido).AsQueryable();
                    break;
                case CAMPOS.NOMEEXTENSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.NomeExtenso).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.NomeExtenso).AsQueryable();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Bancos
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            Populate();

            //DECLARAÇÕES
            List<dynamic> CollectionBancos = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
            var queryTotal = query;

            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = queryTotal.Count();


            // PAGINAÇÃO
            int skipRows = (pageNumber - 1) * pageSize;
            if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                query = query.Skip(skipRows).Take(pageSize);
            else
                pageNumber = 1;

            retorno.PaginaAtual = pageNumber;
            retorno.ItensPorPagina = pageSize;

            // COLEÇÃO DE RETORNO
            if (colecao == 1)
            {
                CollectionBancos = query.Select(e => new
                {
                    Codigo = e.Codigo,
                    NomeReduzido = e.NomeReduzido,
                    NomeExtenso = e.NomeExtenso
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionBancos = query.Select(e => new
                {
                    Codigo = e.Codigo,
                    NomeExtenso = e.NomeExtenso
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionBancos;

            return retorno;
        }

        /// <summary>
        /// Retorna o nome do Banco apartir do Código
        /// </summary>
        /// <param name="CodigoBanco"></param>
        /// <returns></returns>
        public static string Get(string CodigoBanco)
        {
            Populate();
            return ListBancos.Where(e => e.Codigo.Equals(CodigoBanco)).Select(e => e.NomeReduzido).FirstOrDefault<string>();
        }
    }
}