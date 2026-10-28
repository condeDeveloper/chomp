namespace Conde.Chomp;

/// <summary>O que a análise de uma posição achou.</summary>
public readonly record struct Analise(bool PrimeiroGanha, int LancesVencedores, (int Coluna, int Linha)? Primeiro, long Posicoes, long Nos);

/// <summary>
/// Quem ganha. A regra é a de sempre em jogo sem empate: uma posição é vitória se
/// existe uma mordida que deixa o adversário numa posição perdida; é derrota se
/// toda mordida entrega uma posição ganha. A posição com só o veneno é derrota
/// para quem está na vez.
/// </summary>
public sealed class Resolvedor
{
    private readonly Dictionary<long, bool> _memoria = new();

    /// <summary>Quantas posições diferentes a memória guardou.</summary>
    public int Lembradas => _memoria.Count;

    public long Nos { get; private set; }

    /// <summary>Quem está na vez ganha nesta posição?</summary>
    public bool Ganha(Posicao posicao)
    {
        ArgumentNullException.ThrowIfNull(posicao);
        Nos++;

        if (posicao.Vazia) return true;          // o adversário comeu o veneno
        if (posicao.SoOVeneno) return false;     // só sobrou o veneno, e é minha vez

        var chave = posicao.Chave();
        if (_memoria.TryGetValue(chave, out var guardado)) return guardado;

        var ganha = false;
        foreach (var (_, _, resultado) in posicao.Mordidas())
        {
            if (Ganha(resultado)) continue;
            ganha = true;
            break;
        }

        _memoria[chave] = ganha;
        return ganha;
    }

    /// <summary>Todas as mordidas que ganham, em ordem.</summary>
    public List<(int Coluna, int Linha)> LancesVencedores(Posicao posicao)
    {
        ArgumentNullException.ThrowIfNull(posicao);
        var saida = new List<(int, int)>();
        foreach (var (coluna, linha, resultado) in posicao.Mordidas())
            if (!Ganha(resultado))
                saida.Add((coluna, linha));
        return saida;
    }

    /// <summary>A análise completa de uma barra.</summary>
    public Analise Analisa(int colunas, int linhas)
    {
        var inicial = Posicao.Barra(colunas, linhas);
        var ganha = Ganha(inicial);
        var vencedores = LancesVencedores(inicial);
        return new Analise(
            ganha,
            vencedores.Count,
            vencedores.Count > 0 ? vencedores[0] : null,
            _memoria.Count,
            Nos);
    }

    /// <summary>
    /// O mesmo resultado sem memória nenhuma, recalculando tudo toda vez. É lento
    /// de propósito: ele não compartilha a tabela com ninguém, e por isso serve de
    /// juiz do que a versão com memória afirma.
    /// </summary>
    public static bool GanhaSemMemoria(Posicao posicao)
    {
        ArgumentNullException.ThrowIfNull(posicao);
        if (posicao.Vazia) return true;
        if (posicao.SoOVeneno) return false;

        foreach (var (_, _, resultado) in posicao.Mordidas())
            if (!GanhaSemMemoria(resultado))
                return true;

        return false;
    }
}
