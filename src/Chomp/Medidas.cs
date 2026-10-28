using System.Diagnostics;

namespace Conde.Chomp;

/// <summary>
/// As medidas do README. Os testes conferem o que estes métodos devolvem; o
/// medidor só imprime.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaBarra(
        int Colunas, int Linhas, bool PrimeiroGanha, int LancesVencedores, string Primeiro, long Posicoes);

    public readonly record struct LinhaRoubo(
        int Colunas, int Linhas, bool CantoJaGanha, string LanceRoubado, bool Confere);

    public readonly record struct LinhaDuasLinhas(int Altas, int Baixas, bool Ganha);

    public readonly record struct LinhaJuiz(
        int Colunas, int Linhas, long NosComMemoria, long NosSemMemoria, double Vezes, bool Bate);

    /// <summary>Todas as barras até 7 por 7, cada uma analisada do zero.</summary>
    public static List<LinhaBarra> Barras(int ate = 7)
    {
        var linhas = new List<LinhaBarra>();
        for (var c = 1; c <= ate; c++)
            for (var l = 1; l <= ate; l++)
            {
                var motor = new Resolvedor();
                var analise = motor.Analisa(c, l);
                linhas.Add(new LinhaBarra(
                    c, l, analise.PrimeiroGanha, analise.LancesVencedores,
                    analise.Primeiro is null ? "-" : $"{analise.Primeiro.Value.Coluna},{analise.Primeiro.Value.Linha}",
                    analise.Posicoes));
            }
        return linhas;
    }

    /// <summary>
    /// Só as barras quadradas, que são as únicas em que a estratégia é conhecida:
    /// morder o quadrado vizinho do veneno na diagonal e devolver sempre um L
    /// simétrico.
    /// </summary>
    public static List<LinhaBarra> Quadrados(int ate = 7) =>
        Barras(ate).Where(l => l.Colunas == l.Linhas).ToList();

    /// <summary>
    /// Quantas barras têm zero, um ou mais de um lance vencedor. O primeiro lance
    /// ser único é o tipo de coisa que não está em enunciado nenhum e cai da
    /// varredura.
    /// </summary>
    public static (int Barras, int SemLance, int ComUm, int ComMais) LanceUnico(int ate = 7)
    {
        var linhas = Barras(ate);
        return (
            linhas.Count,
            linhas.Count(l => l.LancesVencedores == 0),
            linhas.Count(l => l.LancesVencedores == 1),
            linhas.Count(l => l.LancesVencedores > 1));
    }

    /// <summary>
    /// Em quantas barras morder só o canto mais longe do veneno já ganha, sem
    /// precisar roubar nada.
    /// </summary>
    public static (int Barras, int CantoGanha, int Conferem) OCantoBasta(int ate = 6)
    {
        var linhas = Roubos(ate);
        return (linhas.Count, linhas.Count(l => l.CantoJaGanha), linhas.Count(l => l.Confere));
    }

    /// <summary>O argumento do roubo, conferido barra a barra.</summary>
    public static List<LinhaRoubo> Roubos(int ate = 6)
    {
        var linhas = new List<LinhaRoubo>();
        for (var c = 1; c <= ate; c++)
            for (var l = 1; l <= ate; l++)
            {
                var furto = Roubo.Confere(c, l);
                linhas.Add(new LinhaRoubo(
                    c, l, furto.CantoJaGanha,
                    furto.LanceRoubado is null ? "-" : $"{furto.LanceRoubado.Value.Coluna},{furto.LanceRoubado.Value.Linha}",
                    furto.Confere));
            }
        return linhas;
    }

    /// <summary>As posições de duas linhas em que quem está na vez perde.</summary>
    public static List<LinhaDuasLinhas> PerdedorasDeDuasLinhas(int ate = 9)
    {
        var motor = new Resolvedor();
        return DuasLinhas.Todas(ate, motor)
            .Where(p => !p.Ganha)
            .Select(p => new LinhaDuasLinhas(p.Altas, p.Baixas, p.Ganha))
            .ToList();
    }

    /// <summary>A memória contra a força bruta pura, enquanto a força bruta aguenta.</summary>
    public static List<LinhaJuiz> ContraOJuiz(int ate = 4)
    {
        var linhas = new List<LinhaJuiz>();
        for (var c = 1; c <= ate; c++)
            for (var l = 1; l <= ate; l++)
            {
                var motor = new Resolvedor();
                var comMemoria = motor.Ganha(Posicao.Barra(c, l));
                var nosCom = motor.Nos;

                var contador = new ContadorDeNos();
                var semMemoria = contador.Ganha(Posicao.Barra(c, l));

                linhas.Add(new LinhaJuiz(
                    c, l, nosCom, contador.Nos, (double)contador.Nos / nosCom, comMemoria == semMemoria));
            }
        return linhas;
    }

    /// <summary>Quanto tempo a análise da maior barra leva, para o README não mentir.</summary>
    public static (int Colunas, int Linhas, long Posicoes, double Segundos) AMaiorQueCabe(int lado = 12)
    {
        var motor = new Resolvedor();
        var relogio = Stopwatch.StartNew();
        motor.Ganha(Posicao.Barra(lado, lado));
        relogio.Stop();
        return (lado, lado, motor.Lembradas, relogio.Elapsed.TotalSeconds);
    }

    /// <summary>O mesmo cálculo sem memória, contando os nós abertos.</summary>
    private sealed class ContadorDeNos
    {
        public long Nos { get; private set; }

        public bool Ganha(Posicao posicao)
        {
            Nos++;
            if (posicao.Vazia) return true;
            if (posicao.SoOVeneno) return false;

            foreach (var (_, _, resultado) in posicao.Mordidas())
                if (!Ganha(resultado))
                    return true;

            return false;
        }
    }
}
