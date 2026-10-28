using Conde.Chomp;
using Xunit;

namespace Conde.Chomp.Testes;

public class RouboTestes
{
    [Fact]
    public void OArgumentoConfereEmTodaBarraAteSeisPorSeis()
    {
        var motor = new Resolvedor();
        for (var c = 1; c <= 6; c++)
            for (var l = 1; l <= 6; l++)
                Assert.True(Roubo.Confere(c, l, motor).Confere, $"{c} por {l}");
    }

    [Fact]
    public void MorderSoOCantoSoBastaQuandoUmLadoEhDois()
    {
        var motor = new Resolvedor();
        for (var c = 1; c <= 6; c++)
            for (var l = 1; l <= 6; l++)
            {
                var furto = Roubo.Confere(c, l, motor);
                if (!furto.CantoJaGanha) continue;
                Assert.True(c == 2 || l == 2, $"{c} por {l} não tem lado 2");
            }
    }

    [Fact]
    public void OLanceRoubadoVenceNaBarraInteira()
    {
        var motor = new Resolvedor();
        for (var c = 3; c <= 6; c++)
            for (var l = 3; l <= 6; l++)
            {
                var furto = Roubo.Confere(c, l, motor);
                Assert.NotNull(furto.LanceRoubado);
                var depois = Posicao.Barra(c, l).Morde(furto.LanceRoubado!.Value.Coluna, furto.LanceRoubado.Value.Linha);
                Assert.False(motor.Ganha(depois));
            }
    }

    [Fact]
    public void BarraSemColunaNaoEhPergunta()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Roubo.Confere(0, 3));
    }

    [Fact]
    public void ComDuasLinhasPerdeQuemPegaAEscada()
    {
        var motor = new Resolvedor();
        for (var altas = 0; altas <= 8; altas++)
            Assert.False(motor.Ganha(DuasLinhas.Monta(altas, 1)), $"{altas} altas");
    }

    [Fact]
    public void ARegraDasDuasLinhasNaoFalhaEmNenhumaPosicao()
    {
        var (conferidas, desobedeceram) = DuasLinhas.ARegra(9);
        Assert.Equal(99, conferidas);
        Assert.Equal(0, desobedeceram);
    }

    [Fact]
    public void APosicaoDeDuasLinhasTemAAlturaCerta()
    {
        Assert.Equal("2,2,1", DuasLinhas.Monta(2, 1).ToString());
        Assert.Equal("1", DuasLinhas.Monta(0, 1).ToString());
        Assert.Equal("2,2", DuasLinhas.Monta(2, 0).ToString());
    }

    [Fact]
    public void QualquerPosicaoDeDuasLinhasForaDaEscadaGanha()
    {
        var motor = new Resolvedor();
        foreach (var (altas, baixas, ganha) in DuasLinhas.Todas(7, motor))
            Assert.Equal(baixas != 1, ganha);
    }
}
