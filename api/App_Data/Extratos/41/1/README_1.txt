O QUE ELE FAZ?

Procura no arquivo linhas que contenham "<link href=" e "<script src=" para substituir a referência 
.css ou .js que não forem mínimas (isto é, .min.css e .min.js) para a referência mínima.


EXEMPLO:

Arquivo com:
	<link href="lib/bootstrap/bootstrap.min.css" rel="stylesheet" type="text/css">
	<link href="css/custom.css" rel="stylesheet" type="text/css"/>
	<script src="lib/bootstrap/bootstrap.min.js" type="text/javascript"></script>
	<script src="js/custom.js" type="text/javascript"></script>
	
Fica com:
	<link href="lib/bootstrap/bootstrap.min.css" rel="stylesheet" type="text/css">
	<link href="css/custom.min.css" rel="stylesheet" type="text/css"/>
	<script src="lib/bootstrap/bootstrap.min.js" type="text/javascript"></script>
	<script src="js/custom.min.js" type="text/javascript"></script>


	
COMO USAR?

Com o prompt de comando, fazer a seguinte chamada (no Windows)

referenciaMin <arquivoentrada>.html <arquivosaida>.html


EXEMPLOS:

referenciaMin index.html indexMin.html // ambos se encontram na mesma pasta do programa
referenciaMin "C:\Projeto\index.html" indexMin.html // o arquivo de saída será salvo na mesma pasta do programa
referenciaMin "C:\Projeto\index.html" "C:\Novo Projeto\index.html"



# Desenvolvedor

Deivid Marinho - deividgfmarinho@gmail.com
