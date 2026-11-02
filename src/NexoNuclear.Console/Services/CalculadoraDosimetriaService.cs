namespace NexoNuclear.Services;

/// <summary>
/// Estimativas educativas por contexto, sem classificar risco ou conformidade regulatória.
/// Fontes, hipóteses e limitações: docs/fontes-e-limitacoes.md.
/// </summary>
public class CalculadoraDosimetriaService
{
    // FDA: valores típicos de dose efetiva, não doses individuais medidas.
    public const double DoseRaioXToraxPaMicrosievert = 20.0;
    public const double DoseTomografiaAbdomenMicrosievert = 8000.0;

    // Hipótese didática fixa, não medição nem estimativa de uma rota real.
    public const double DoseHoraVooComercial = 5.0;

    public ResultadoSimulacao Simular(int radiografiasToraxPa, int tomografiasAbdomen,
        int horasVoo, int bananasPorSemana)
    {
        if (radiografiasToraxPa < 0 || tomografiasAbdomen < 0 || horasVoo < 0 || bananasPorSemana < 0)
            throw new ArgumentException("As quantidades devem ser inteiros não negativos.");

        return new ResultadoSimulacao(
            radiografiasToraxPa * DoseRaioXToraxPaMicrosievert +
                tomografiasAbdomen * DoseTomografiaAbdomenMicrosievert,
            horasVoo * DoseHoraVooComercial,
            bananasPorSemana * 52L);
    }
}

// Intencionalmente sem dose total entre contextos e sem porcentagem de limite.
// Alimentação é contagem ilustrativa, não dose acumulada por ingestão de K-40.
public sealed record ResultadoSimulacao(
    double DoseMedicaMicrosievert, double DoseVoosMicrosievert, long BananasPorAno);
