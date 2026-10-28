using Conde.Chomp;

var comando = args.Length > 0 ? args[0] : "tudo";

if (comando is "barras" or "tudo") Barras();
if (comando is "quadrados" or "tudo") Quadrados();
if (comando is "roubo" or "tudo") Roubos();
if (comando is "duaslinhas" or "tudo") DuasLinhasTabela();
if (comando is "juiz" or "tudo") Juiz();
if (comando is "maior" or "tudo") Maior();

static void Barras()
{
    Console.WriteLine();
    Console.WriteLine("TODA BARRA, analisada do zero. Quem comeca ganha em todas, menos no quadrado de um.");
    Console.WriteLine();
    Console.WriteLine($"  {"colunas",8} {"linhas",7} {"quem ganha",11} {"lances que vencem",18} {"o primeiro",11} {"posicoes",10}");

    foreach (var l in Medidas.Barras())
        Console.WriteLine($"  {l.Colunas,8} {l.Linhas,7} {(l.PrimeiroGanha ? "o primeiro" : "o segundo"),11} {l.LancesVencedores,18} {l.Primeiro,11} {l.Posicoes,10:n0}");

    Console.WriteLine();
}

static void Quadrados()
{
    Console.WriteLine();
    Console.WriteLine("AS BARRAS QUADRADAS, as unicas com estrategia conhecida.");
    Console.WriteLine();
    Console.WriteLine($"  {"lado",6} {"lances que vencem",18} {"o primeiro",11} {"posicoes",10}");

    foreach (var l in Medidas.Quadrados())
        Console.WriteLine($"  {l.Colunas,6} {l.LancesVencedores,18} {l.Primeiro,11} {l.Posicoes,10:n0}");

    Console.WriteLine();
}

static void Roubos()
{
    Console.WriteLine();
    Console.WriteLine("O ARGUMENTO DO ROUBO, conferido barra a barra.");
    Console.WriteLine();
    Console.WriteLine($"  {"colunas",8} {"linhas",7} {"o canto ja ganha",17} {"lance roubado",14} {"confere",8}");

    foreach (var l in Medidas.Roubos())
        Console.WriteLine($"  {l.Colunas,8} {l.Linhas,7} {(l.CantoJaGanha ? "sim" : "nao"),17} {l.LanceRoubado,14} {(l.Confere ? "sim" : "NAO"),8}");

    Console.WriteLine();
}

static void DuasLinhasTabela()
{
    Console.WriteLine();
    Console.WriteLine("CHOMP DE DUAS LINHAS: as posicoes em que quem esta na vez PERDE.");
    Console.WriteLine();
    Console.WriteLine($"  {"altura 2",9} {"altura 1",9}");

    foreach (var l in Medidas.PerdedorasDeDuasLinhas())
        Console.WriteLine($"  {l.Altas,9} {l.Baixas,9}");

    var (conferidas, desobedeceram) = DuasLinhas.ARegra(9);
    Console.WriteLine();
    Console.WriteLine($"  posicoes conferidas contra a regra: {conferidas}");
    Console.WriteLine($"  desobedeceram:                      {desobedeceram}");
    Console.WriteLine();
}

static void Juiz()
{
    Console.WriteLine();
    Console.WriteLine("A MEMORIA CONTRA A FORCA BRUTA PURA, enquanto a forca bruta aguenta.");
    Console.WriteLine();
    Console.WriteLine($"  {"colunas",8} {"linhas",7} {"nos com memoria",16} {"nos sem memoria",16} {"razao",9} {"bate",6}");

    foreach (var l in Medidas.ContraOJuiz())
        Console.WriteLine($"  {l.Colunas,8} {l.Linhas,7} {l.NosComMemoria,16:n0} {l.NosSemMemoria,16:n0} {l.Vezes,8:0.0}x {(l.Bate ? "sim" : "NAO"),6}");

    Console.WriteLine();
}

static void Maior()
{
    Console.WriteLine();
    var (colunas, linhas, posicoes, segundos) = Medidas.AMaiorQueCabe();
    Console.WriteLine($"A MAIOR QUE CABE: {colunas} por {linhas}");
    Console.WriteLine();
    Console.WriteLine($"  posicoes guardadas  {posicoes,12:n0}");
    Console.WriteLine($"  segundos            {segundos,12:0.00}");
    Console.WriteLine();
}
