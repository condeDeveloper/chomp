using Conde.Chomp;
using Xunit;

namespace Conde.Chomp.Testes;

public class ResolvedorTestes
{
    [Fact]
    public void OQuadradoDeUmEhAUnicaBarraPerdida()
    {
        var motor = new Resolvedor();
        Assert.False(motor.Ganha(Posicao.Barra(1, 1)));

        for (var c = 1; c <= 6; c++)
            for (var l = 1; l <= 6; l++)
                if (c != 1 || l != 1)
                    Assert.True(motor.Ganha(Posicao.Barra(c, l)), $"{c} por {l}");
    }

    [Fact]
    public void AMesaVaziaEhVitoriaDeQuemEstaNaVez()
    {
        // Quem está na vez não joga: o adversário acabou de comer o veneno.
        Assert.True(new Resolvedor().Ganha(new Posicao()));
    }

    [Fact]
    public void ComMemoriaESemMemoriaDaoAMesmaResposta()
    {
        var motor = new Resolvedor();
        for (var c = 1; c <= 4; c++)
            for (var l = 1; l <= 4; l++)
            {
                var barra = Posicao.Barra(c, l);
                Assert.Equal(Resolvedor.GanhaSemMemoria(barra), motor.Ganha(barra));
            }
    }

    [Fact]
    public void OLanceVencedorLevaMesmoAUmaPosicaoPerdida()
    {
        var motor = new Resolvedor();
        for (var c = 1; c <= 5; c++)
            for (var l = 1; l <= 5; l++)
            {
                var barra = Posicao.Barra(c, l);
                foreach (var (coluna, linha) in motor.LancesVencedores(barra))
                    Assert.False(motor.Ganha(barra.Morde(coluna, linha)));
            }
    }

    [Fact]
    public void EmTodaBarraQueGanhaExisteExatamenteUmLanceVencedor()
    {
        var motor = new Resolvedor();
        for (var c = 1; c <= 6; c++)
            for (var l = 1; l <= 6; l++)
            {
                if (c == 1 && l == 1) continue;
                Assert.Single(motor.LancesVencedores(Posicao.Barra(c, l)));
            }
    }

    [Fact]
    public void NoQuadradoOLanceVencedorEhSempreOVizinhoDoVenenoNaDiagonal()
    {
        var motor = new Resolvedor();
        for (var lado = 2; lado <= 7; lado++)
            Assert.Equal((1, 1), motor.LancesVencedores(Posicao.Barra(lado, lado)).Single());
    }

    [Fact]
    public void NaBarraDeUmaColunaOLanceEhComerTudoMenosOVeneno()
    {
        var motor = new Resolvedor();
        for (var linhas = 2; linhas <= 7; linhas++)
            Assert.Equal((0, 1), motor.LancesVencedores(Posicao.Barra(1, linhas)).Single());
    }

    [Fact]
    public void AMemoriaGuardaMenosPosicoesQueOsNosAbertos()
    {
        var motor = new Resolvedor();
        motor.Ganha(Posicao.Barra(5, 5));
        Assert.True(motor.Lembradas < motor.Nos);
    }

    [Fact]
    public void AAnaliseDaBarraTrazOPrimeiroLance()
    {
        var analise = new Resolvedor().Analisa(4, 4);
        Assert.True(analise.PrimeiroGanha);
        Assert.Equal(1, analise.LancesVencedores);
        Assert.Equal((1, 1), analise.Primeiro);
    }

    [Fact]
    public void OQuadradoDeUmNaoTemLanceVencedor()
    {
        var analise = new Resolvedor().Analisa(1, 1);
        Assert.False(analise.PrimeiroGanha);
        Assert.Equal(0, analise.LancesVencedores);
        Assert.Null(analise.Primeiro);
    }
}
