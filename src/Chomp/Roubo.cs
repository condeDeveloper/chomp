namespace Conde.Chomp;

/// <summary>O que a conferência do argumento de roubo achou numa barra.</summary>
public readonly record struct Furto(
    int Colunas, int Linhas, bool CantoJaGanha, (int Coluna, int Linha)? LanceRoubado, bool Confere);

/// <summary>
/// O argumento do roubo de estratégia, de 1952: em qualquer barra maior que um
/// quadrado, o primeiro a jogar ganha. A prova é curta e não constrói lance
/// nenhum, e é exatamente isso que este arquivo torna palpável.
/// </summary>
public static class Roubo
{
    /// <summary>
    /// Confere o argumento numa barra, passo a passo:
    ///
    /// morder só o canto mais longe do veneno deixa uma posição A. Se A for
    /// perdida para o adversário, o primeiro já ganhou. Se A for ganha, existe
    /// uma mordida vencedora M a partir de A; e aí M, feita direto na barra
    /// inteira, também vence, porque morder o canto antes não muda nada do que M
    /// tira. O primeiro ganha nos dois casos, e em nenhum deles se sabe qual é o
    /// lance antes de calcular.
    /// </summary>
    public static Furto Confere(int colunas, int linhas, Resolvedor? resolvedor = null)
    {
        if (colunas < 1 || linhas < 1) throw new ArgumentOutOfRangeException(nameof(colunas));

        var motor = resolvedor ?? new Resolvedor();
        var barra = Posicao.Barra(colunas, linhas);
        if (barra.SoOVeneno) return new Furto(colunas, linhas, false, null, !motor.Ganha(barra));

        var canto = barra.Morde(colunas - 1, linhas - 1);
        if (!motor.Ganha(canto))
            return new Furto(colunas, linhas, true, (colunas - 1, linhas - 1), motor.Ganha(barra));

        // O adversário teria resposta; então roubamos a resposta dele.
        var roubado = motor.LancesVencedores(canto).First();
        var depoisDoRoubo = barra.Morde(roubado.Coluna, roubado.Linha);
        var confere = !motor.Ganha(depoisDoRoubo) && motor.Ganha(barra);

        return new Furto(colunas, linhas, false, roubado, confere);
    }
}

/// <summary>
/// Chomp de duas linhas, que é o único tamanho em que a estratégia cabe numa
/// frase. Aqui a frase é CONFERIDA contra a varredura, e não assumida.
/// </summary>
public static class DuasLinhas
{
    /// <summary>
    /// A posição de duas linhas com <paramref name="altas"/> colunas de altura 2
    /// e <paramref name="baixas"/> colunas de altura 1.
    /// </summary>
    public static Posicao Monta(int altas, int baixas)
    {
        var alturas = new int[altas + baixas];
        for (var i = 0; i < altas; i++) alturas[i] = 2;
        for (var i = altas; i < alturas.Length; i++) alturas[i] = 1;
        return new Posicao(alturas);
    }

    /// <summary>
    /// Todas as posições de duas linhas até um tamanho, separadas em quem perde e
    /// quem ganha para quem está na vez.
    /// </summary>
    public static List<(int Altas, int Baixas, bool Ganha)> Todas(int ate, Resolvedor? resolvedor = null)
    {
        var motor = resolvedor ?? new Resolvedor();
        var saida = new List<(int, int, bool)>();
        for (var altas = 0; altas <= ate; altas++)
            for (var baixas = 0; baixas <= ate; baixas++)
            {
                if (altas == 0 && baixas == 0) continue;
                saida.Add((altas, baixas, motor.Ganha(Monta(altas, baixas))));
            }
        return saida;
    }

    /// <summary>
    /// A regra: com duas linhas, quem está na vez perde exatamente quando a linha
    /// de baixo tem UM quadrado a mais que a de cima — isto é, quando existe
    /// exatamente UMA coluna de altura 1. Devolve quantas posições foram
    /// conferidas e quantas desobedeceram.
    /// </summary>
    public static (int Conferidas, int Desobedeceram) ARegra(int ate, Resolvedor? resolvedor = null)
    {
        var desobedeceram = 0;
        var todas = Todas(ate, resolvedor);
        foreach (var (_, baixas, ganha) in todas)
        {
            var aRegraDiz = baixas != 1;
            if (ganha != aRegraDiz) desobedeceram++;
        }
        return (todas.Count, desobedeceram);
    }
}
